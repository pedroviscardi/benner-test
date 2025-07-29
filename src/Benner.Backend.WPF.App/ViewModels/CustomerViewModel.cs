using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Benner.Backend.Application.UseCases.Customer.Commands;
using Benner.Backend.Application.UseCases.Customer.Queries;
using Benner.Backend.Domain.Entities;
using Benner.Backend.Domain.ValueObjects;
using Benner.Backend.Shared.Common;
using Benner.Backend.WPF.App.Commands;

namespace Benner.Backend.WPF.App.ViewModels
{
    public class CustomerViewModel : BaseViewModel
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;
        private DateTime _birthDate = DateTime.Today.AddYears(-18);
        private string _city;
        private string _document;
        private string _email;
        private bool _isLoading;
        private string _name;
        private string _neighborhood;
        private string _number;
        private string _phone;
        private string _postalCode;

        private Customer _selectedCustomer;
        private string _state;
        private string _statusMessage;
        private string _street;

        public CustomerViewModel(ICommandBus commandBus, IQueryBus queryBus)
        {
            _commandBus = commandBus ?? throw new ArgumentNullException(nameof(commandBus));
            _queryBus = queryBus ?? throw new ArgumentNullException(nameof(queryBus));

            Customers = new ObservableCollection<Customer>();
            LoadCustomersCommand = new AsyncRelayCommand(LoadCustomersAsync);
            SaveCustomerCommand = new AsyncRelayCommand(SaveCustomerAsync, CanSaveCustomer);
            DeleteCustomerCommand = new AsyncRelayCommand(DeleteCustomerAsync, CanDeleteCustomer);
            NewCustomerCommand = new RelayCommand(NewCustomer);

            _ = Task.Run(LoadCustomersAsync);
        }

        public ObservableCollection<Customer> Customers { get; }

        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                if (SetProperty(ref _selectedCustomer, value))
                {
                    LoadCustomerDetails();
                }
            }
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        public string Document
        {
            get => _document;
            set => SetProperty(ref _document, value);
        }

        public DateTime BirthDate
        {
            get => _birthDate;
            set => SetProperty(ref _birthDate, value);
        }

        public string Street
        {
            get => _street;
            set => SetProperty(ref _street, value);
        }

        public string Number
        {
            get => _number;
            set => SetProperty(ref _number, value);
        }

        public string Neighborhood
        {
            get => _neighborhood;
            set => SetProperty(ref _neighborhood, value);
        }

        public string City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }

        public string State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }

        public string PostalCode
        {
            get => _postalCode;
            set => SetProperty(ref _postalCode, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand LoadCustomersCommand { get; }
        public ICommand SaveCustomerCommand { get; }
        public ICommand DeleteCustomerCommand { get; }
        public ICommand NewCustomerCommand { get; }

        private async Task LoadCustomersAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Carregando clientes...";

                var query = new GetAllCustomersQuery();
                var result = await _queryBus.ExecuteAsync<GetAllCustomersQuery, Result<IEnumerable<Customer>>>(query);

                if (result.IsSuccess)
                {
                    Customers.Clear();
                    foreach (var customer in result.Data)
                    {
                        Customers.Add(customer);
                    }

                    StatusMessage = $"{Customers.Count} clientes carregados";
                }
                else
                {
                    StatusMessage = $"Erro: {result.Error}";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro ao carregar clientes: {ex.Message}";
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
                StatusMessage = "Salvando cliente...";

                var address = new Address(Street, Number, Neighborhood, City, State, PostalCode);

                if (SelectedCustomer == null)
                {
                    // Criar novo cliente
                    var createCommand = new CreateCustomerCommand(Name, Email, Phone, Document, BirthDate, address);
                    var result = await _commandBus.ExecuteAsync<CreateCustomerCommand, Result<Customer>>(createCommand);

                    if (result.IsSuccess)
                    {
                        Customers.Add(result.Data);
                        SelectedCustomer = result.Data;
                        StatusMessage = "Cliente criado com sucesso!";
                    }
                    else
                    {
                        StatusMessage = $"Erro: {result.Error}";
                    }
                }
                else
                {
                    // Atualizar cliente existente
                    var updateCommand = new UpdateCustomerCommand(SelectedCustomer.Id, Name, Email, Phone, BirthDate, address);
                    var result = await _commandBus.ExecuteAsync<UpdateCustomerCommand, Result<Customer>>(updateCommand);

                    if (result.IsSuccess)
                    {
                        // Atualizar na lista
                        var index = Customers.IndexOf(SelectedCustomer);
                        if (index >= 0)
                        {
                            Customers[index] = result.Data;
                            SelectedCustomer = result.Data;
                        }

                        StatusMessage = "Cliente atualizado com sucesso!";
                    }
                    else
                    {
                        StatusMessage = $"Erro: {result.Error}";
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro ao salvar cliente: {ex.Message}";
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
                StatusMessage = "Excluindo cliente...";

                var deleteCommand = new DeleteCustomerCommand(SelectedCustomer.Id);
                var result = await _commandBus.ExecuteAsync<DeleteCustomerCommand, Result<bool>>(deleteCommand);

                if (result.IsSuccess && result.Data)
                {
                    Customers.Remove(SelectedCustomer);
                    NewCustomer();
                    StatusMessage = "Cliente excluído com sucesso!";
                }
                else
                {
                    StatusMessage = $"Erro: {result.Error}";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro ao excluir cliente: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void NewCustomer()
        {
            SelectedCustomer = null;
            Name = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            Document = string.Empty;
            BirthDate = DateTime.Today.AddYears(-18);
            Street = string.Empty;
            Number = string.Empty;
            Neighborhood = string.Empty;
            City = string.Empty;
            State = string.Empty;
            PostalCode = string.Empty;
            StatusMessage = "Novo cliente";
        }

        private void LoadCustomerDetails()
        {
            if (SelectedCustomer == null) return;

            Name = SelectedCustomer.Name;
            Email = SelectedCustomer.Email;
            Phone = SelectedCustomer.Phone;
            Document = SelectedCustomer.Document;
            BirthDate = SelectedCustomer.BirthDate;

            if (SelectedCustomer.Address != null)
            {
                Street = SelectedCustomer.Address.Street;
                Number = SelectedCustomer.Address.Number;
                Neighborhood = SelectedCustomer.Address.Neighborhood;
                City = SelectedCustomer.Address.City;
                State = SelectedCustomer.Address.State;
                PostalCode = SelectedCustomer.Address.PostalCode;
            }
        }

        private bool CanSaveCustomer()
        {
            return !IsLoading &&
                   !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Phone) &&
                   !string.IsNullOrWhiteSpace(Document);
        }

        private bool CanDeleteCustomer()
        {
            return !IsLoading && SelectedCustomer != null;
        }
    }
}