using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Benner.Backend.Application.UseCases.Customer.Commands;
using Benner.Backend.Application.UseCases.Customer.Queries;
using Benner.Backend.Domain.Entities;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;
using Benner.Backend.Shared.Queries;
using Benner.Backend.WPF.App.Commands;
using ICommand = System.Windows.Input.ICommand;

namespace Benner.Backend.WPF.App.ViewModels
{
    public class CustomerViewModel : BaseViewModel
    {
        private readonly ICommandHandler<CreateCustomerCommand, Result<Customer>> _createCustomerHandler;
        private readonly ICommandHandler<DeleteCustomerCommand, Result<bool>> _deleteCustomerHandler;
        private readonly IQueryHandler<GetAllCustomersQuery, Result<IEnumerable<Customer>>> _getAllCustomersHandler;
        private readonly ICommandHandler<UpdateCustomerCommand, Result<Customer>> _updateCustomerHandler;
        private Customer _currentCustomer;

        private ObservableCollection<Customer> _customers;
        private string _errorMessage;
        private ObservableCollection<Customer> _filteredCustomers;
        private bool _isEditing;
        private bool _isLoading;
        private string _searchText;
        private Customer _selectedCustomer;
        private string _successMessage;

        public CustomerViewModel(
            IQueryHandler<GetAllCustomersQuery, Result<IEnumerable<Customer>>> getAllCustomersHandler,
            ICommandHandler<CreateCustomerCommand, Result<Customer>> createCustomerHandler,
            ICommandHandler<UpdateCustomerCommand, Result<Customer>> updateCustomerHandler,
            ICommandHandler<DeleteCustomerCommand, Result<bool>> deleteCustomerHandler)
        {
            _getAllCustomersHandler = getAllCustomersHandler;
            _createCustomerHandler = createCustomerHandler;
            _updateCustomerHandler = updateCustomerHandler;
            _deleteCustomerHandler = deleteCustomerHandler;

            Customers = new ObservableCollection<Customer>();
            FilteredCustomers = new ObservableCollection<Customer>();

            LoadCustomersCommand = new AsyncRelayCommand(LoadCustomersAsync);
            SaveCustomerCommand = new AsyncRelayCommand(SaveCustomerAsync, CanSaveCustomer);
            DeleteCustomerCommand = new AsyncRelayCommand(DeleteCustomerAsync, CanDeleteCustomer);
            NewCustomerCommand = new RelayCommand(NewCustomer);
            EditCustomerCommand = new RelayCommand(EditCustomer, CanEditCustomer);
            CancelCommand = new RelayCommand(Cancel);
            SearchCommand = new RelayCommand(PerformSearch);

            CurrentCustomer = Customer.CreateEmpty();

            _ = LoadCustomersAsync();
        }

        #region Properties

        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }

        public ObservableCollection<Customer> FilteredCustomers
        {
            get => _filteredCustomers;
            set => SetProperty(ref _filteredCustomers, value);
        }

        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                if (SetProperty(ref _selectedCustomer, value))
                {
                    if (EditCustomerCommand is RelayCommand editCommand)
                        editCommand.RaiseCanExecuteChanged();
                    if (DeleteCustomerCommand is AsyncRelayCommand deleteCommand)
                        deleteCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public Customer CurrentCustomer
        {
            get => _currentCustomer;
            set
            {
                if (SetProperty(ref _currentCustomer, value))
                {
                    if (SaveCustomerCommand is AsyncRelayCommand saveCommand)
                    {
                        saveCommand.RaiseCanExecuteChanged();
                    }
                }
            }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                if (SetProperty(ref _isEditing, value))
                {
                    if (EditCustomerCommand is RelayCommand editCommand)
                        editCommand.RaiseCanExecuteChanged();
                    if (DeleteCustomerCommand is AsyncRelayCommand deleteCommand)
                        deleteCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public string SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    PerformSearch();
                }
            }
        }

        #endregion

        #region Commands

        public ICommand LoadCustomersCommand { get; }
        public ICommand SaveCustomerCommand { get; }
        public ICommand DeleteCustomerCommand { get; }
        public ICommand NewCustomerCommand { get; }
        public ICommand EditCustomerCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SearchCommand { get; }

        #endregion

        #region Command Methods

        private async Task LoadCustomersAsync()
        {
            try
            {
                IsLoading = true;
                ClearMessages();

                var result = await _getAllCustomersHandler.HandleAsync(new GetAllCustomersQuery());
                if (result.IsSuccess && result.Data != null)
                {
                    Customers.Clear();
                    foreach (var customer in result.Data)
                    {
                        Customers.Add(customer);
                    }

                    PerformSearch();
                }
                else
                {
                    ErrorMessage = result.Error ?? "Erro ao carregar clientes";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao carregar clientes: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SaveCustomerAsync()
        {
            try
            {
                IsLoading = true;
                ClearMessages();

                Result<Customer> result;

                if (IsEditing)
                {
                    var command = new UpdateCustomerCommand(
                        CurrentCustomer.Id,
                        CurrentCustomer.Name,
                        CurrentCustomer.Email,
                        CurrentCustomer.Phone,
                        CurrentCustomer.BirthDate,
                        CurrentCustomer.Address);

                    result = await _updateCustomerHandler.HandleAsync(command);
                }
                else
                {
                    var command = new CreateCustomerCommand(
                        CurrentCustomer.Name,
                        CurrentCustomer.Email,
                        CurrentCustomer.Phone,
                        CurrentCustomer.Document,
                        CurrentCustomer.BirthDate,
                        CurrentCustomer.Address);

                    result = await _createCustomerHandler.HandleAsync(command);
                }

                if (result.IsSuccess)
                {
                    SuccessMessage = IsEditing ? "Cliente atualizado com sucesso!" : "Cliente criado com sucesso!";
                    await LoadCustomersAsync();
                    Cancel();
                }
                else
                {
                    ErrorMessage = result.Error;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao salvar cliente: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeleteCustomerAsync()
        {
            if (SelectedCustomer == null) return;

            try
            {
                IsLoading = true;
                ClearMessages();

                var command = new DeleteCustomerCommand(SelectedCustomer.Id);
                var result = await _deleteCustomerHandler.HandleAsync(command);

                if (result.IsSuccess)
                {
                    SuccessMessage = "Cliente excluído com sucesso!";
                    await LoadCustomersAsync();
                    SelectedCustomer = null;
                }
                else
                {
                    ErrorMessage = result.Error;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao excluir cliente: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void NewCustomer()
        {
            CurrentCustomer = Customer.CreateEmpty();
            IsEditing = false;
            ClearMessages();
        }

        private void EditCustomer()
        {
            if (SelectedCustomer == null) return;

            CurrentCustomer = new Customer(
                SelectedCustomer.Name,
                SelectedCustomer.Email,
                SelectedCustomer.Phone,
                SelectedCustomer.Document,
                SelectedCustomer.BirthDate,
                SelectedCustomer.Address);

            typeof(Customer).GetProperty("Id")?.SetValue(CurrentCustomer, SelectedCustomer.Id);

            IsEditing = true;
            ClearMessages();
        }

        private void Cancel()
        {
            CurrentCustomer = Customer.CreateEmpty();
            IsEditing = false;
            ClearMessages();
        }

        private void PerformSearch()
        {
            FilteredCustomers.Clear();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                foreach (var customer in Customers)
                {
                    FilteredCustomers.Add(customer);
                }
            }
            else
            {
                var searchLower = SearchText.ToLower();
                var filtered = Customers.Where(c =>
                    c.Name.ToLower().Contains(searchLower) ||
                    c.Email.ToLower().Contains(searchLower) ||
                    c.Phone.Contains(SearchText) ||
                    c.Document.Contains(SearchText));

                foreach (var customer in filtered)
                {
                    FilteredCustomers.Add(customer);
                }
            }
        }

        #endregion

        #region Helper Methods

        private bool CanSaveCustomer()
        {
            return CurrentCustomer != null &&
                   !string.IsNullOrWhiteSpace(CurrentCustomer.Name) &&
                   !string.IsNullOrWhiteSpace(CurrentCustomer.Email) &&
                   !string.IsNullOrWhiteSpace(CurrentCustomer.Phone) &&
                   !string.IsNullOrWhiteSpace(CurrentCustomer.Document) &&
                   !IsLoading;
        }

        private bool CanDeleteCustomer()
        {
            return SelectedCustomer != null && !IsEditing && !IsLoading;
        }

        private bool CanEditCustomer()
        {
            return SelectedCustomer != null && !IsEditing && !IsLoading;
        }

        private void ClearMessages()
        {
            ErrorMessage = null;
            SuccessMessage = null;
        }

        #endregion
    }
}