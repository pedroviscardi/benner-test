using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Benner.Backend.Application.UseCases.Order.Commands;
using Benner.Backend.Application.UseCases.Order.Queries;
using Benner.Backend.Domain.Entities;
using Benner.Backend.Domain.Enumerators;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;
using Benner.Backend.Shared.Queries;
using Benner.Backend.WPF.App.Commands;
using ICommand = System.Windows.Input.ICommand;

namespace Benner.Backend.WPF.App.ViewModels
{
    public class OrderViewModel : BaseViewModel
    {
        private readonly ICommandHandler<AddOrderItemCommand, Result<Order>> _addOrderItemHandler;
        private readonly ICommandHandler<CancelOrderCommand, Result<Order>> _cancelOrderHandler;
        private readonly ICommandHandler<ConfirmOrderCommand, Result<Order>> _confirmOrderHandler;
        private readonly ICommandHandler<CreateOrderCommand, Result<Order>> _createOrderHandler;
        private readonly IQueryHandler<GetAllOrdersQuery, Result<IEnumerable<Order>>> _getAllOrdersHandler;
        private readonly ICommandHandler<MarkAsDeliveredCommand, Result<Order>> _markAsDeliveredHandler;
        private readonly ICommandHandler<RemoveOrderItemCommand, Result<Order>> _removeOrderItemHandler;
        private string _errorMessage;
        private ObservableCollection<Order> _filteredOrders;
        private bool _isLoading;

        private ObservableCollection<Order> _orders;
        private string _searchText;
        private Order _selectedOrder;
        private OrderStatus? _statusFilter;
        private string _successMessage;

        public OrderViewModel(
            IQueryHandler<GetAllOrdersQuery, Result<IEnumerable<Order>>> getAllOrdersHandler,
            ICommandHandler<CreateOrderCommand, Result<Order>> createOrderHandler,
            ICommandHandler<AddOrderItemCommand, Result<Order>> addOrderItemHandler,
            ICommandHandler<RemoveOrderItemCommand, Result<Order>> removeOrderItemHandler,
            ICommandHandler<ConfirmOrderCommand, Result<Order>> confirmOrderHandler,
            ICommandHandler<CancelOrderCommand, Result<Order>> cancelOrderHandler,
            ICommandHandler<MarkAsDeliveredCommand, Result<Order>> markAsDeliveredHandler)
        {
            _getAllOrdersHandler = getAllOrdersHandler;
            _createOrderHandler = createOrderHandler;
            _addOrderItemHandler = addOrderItemHandler;
            _removeOrderItemHandler = removeOrderItemHandler;
            _confirmOrderHandler = confirmOrderHandler;
            _cancelOrderHandler = cancelOrderHandler;
            _markAsDeliveredHandler = markAsDeliveredHandler;

            Orders = new ObservableCollection<Order>();
            FilteredOrders = new ObservableCollection<Order>();

            LoadOrdersCommand = new AsyncRelayCommand(LoadOrdersAsync);
            ConfirmOrderCommand = new AsyncRelayCommand(ConfirmOrderAsync, CanConfirmOrder);
            CancelOrderCommand = new AsyncRelayCommand(CancelOrderAsync, CanCancelOrder);
            MarkAsDeliveredCommand = new AsyncRelayCommand(MarkAsDeliveredAsync, CanMarkAsDelivered);
            SearchCommand = new RelayCommand(PerformSearch);
            FilterByStatusCommand = new RelayCommand(ApplyStatusFilter);

            _ = LoadOrdersAsync();
        }

        #region Properties

        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set => SetProperty(ref _orders, value);
        }

        public ObservableCollection<Order> FilteredOrders
        {
            get => _filteredOrders;
            set => SetProperty(ref _filteredOrders, value);
        }

        public Order SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                if (SetProperty(ref _selectedOrder, value))
                {
                    if (ConfirmOrderCommand is AsyncRelayCommand confirmCommand)
                        confirmCommand.RaiseCanExecuteChanged();
                    if (CancelOrderCommand is AsyncRelayCommand cancelCommand)
                        cancelCommand.RaiseCanExecuteChanged();
                    if (MarkAsDeliveredCommand is AsyncRelayCommand markCommand)
                        markCommand.RaiseCanExecuteChanged();
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

        public OrderStatus? StatusFilter
        {
            get => _statusFilter;
            set
            {
                if (SetProperty(ref _statusFilter, value))
                {
                    ApplyStatusFilter();
                }
            }
        }

        public IEnumerable<OrderStatus> AvailableStatuses => Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>();

        #endregion

        #region Commands

        public ICommand LoadOrdersCommand { get; }
        public ICommand ConfirmOrderCommand { get; }
        public ICommand CancelOrderCommand { get; }
        public ICommand MarkAsDeliveredCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand FilterByStatusCommand { get; }

        #endregion

        #region Command Methods

        private async Task LoadOrdersAsync()
        {
            try
            {
                IsLoading = true;
                ClearMessages();

                var result = await _getAllOrdersHandler.HandleAsync(new GetAllOrdersQuery());
                if (result.IsSuccess && result.Data != null)
                {
                    Orders.Clear();
                    foreach (var order in result.Data.OrderByDescending(o => o.CreatedAt))
                    {
                        Orders.Add(order);
                    }

                    PerformSearch();
                }
                else
                {
                    ErrorMessage = result.Error ?? "Erro ao carregar pedidos";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao carregar pedidos: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ConfirmOrderAsync()
        {
            if (SelectedOrder == null) return;

            try
            {
                IsLoading = true;
                ClearMessages();

                var command = new ConfirmOrderCommand(SelectedOrder.Id);
                var result = await _confirmOrderHandler.HandleAsync(command);

                if (result.IsSuccess)
                {
                    SuccessMessage = "Pedido confirmado com sucesso!";
                    await LoadOrdersAsync();
                }
                else
                {
                    ErrorMessage = result.Error;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao confirmar pedido: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task CancelOrderAsync()
        {
            if (SelectedOrder == null) return;

            try
            {
                IsLoading = true;
                ClearMessages();

                var command = new CancelOrderCommand(SelectedOrder.Id, "Cancelado pelo usuário");
                var result = await _cancelOrderHandler.HandleAsync(command);

                if (result.IsSuccess)
                {
                    SuccessMessage = "Pedido cancelado com sucesso!";
                    await LoadOrdersAsync();
                }
                else
                {
                    ErrorMessage = result.Error;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao cancelar pedido: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task MarkAsDeliveredAsync()
        {
            if (SelectedOrder == null) return;

            try
            {
                IsLoading = true;
                ClearMessages();

                var command = new MarkAsDeliveredCommand(SelectedOrder.Id);
                var result = await _markAsDeliveredHandler.HandleAsync(command);

                if (result.IsSuccess)
                {
                    SuccessMessage = "Pedido marcado como entregue!";
                    await LoadOrdersAsync();
                }
                else
                {
                    ErrorMessage = result.Error;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao marcar como entregue: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void PerformSearch()
        {
            var filtered = Orders.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchLower = SearchText.ToLower();
                filtered = filtered.Where(o =>
                    o.Id.ToString().Contains(SearchText) ||
                    o.CustomerId.ToString().Contains(SearchText) ||
                    o.Status.ToString().ToLower().Contains(searchLower));
            }

            if (StatusFilter.HasValue)
            {
                filtered = filtered.Where(o => o.Status == StatusFilter.Value);
            }

            FilteredOrders.Clear();
            foreach (var order in filtered)
            {
                FilteredOrders.Add(order);
            }
        }

        private void ApplyStatusFilter()
        {
            PerformSearch();
        }

        #endregion

        #region Helper Methods

        private bool CanConfirmOrder()
        {
            return SelectedOrder != null &&
                   SelectedOrder.Status == OrderStatus.Pending &&
                   !IsLoading;
        }

        private bool CanCancelOrder()
        {
            return SelectedOrder != null &&
                   (SelectedOrder.Status == OrderStatus.Pending || SelectedOrder.Status == OrderStatus.Confirmed) &&
                   !IsLoading;
        }

        private bool CanMarkAsDelivered()
        {
            return SelectedOrder != null &&
                   SelectedOrder.Status == OrderStatus.Confirmed &&
                   !IsLoading;
        }

        private void ClearMessages()
        {
            ErrorMessage = null;
            SuccessMessage = null;
        }

        #endregion
    }
}