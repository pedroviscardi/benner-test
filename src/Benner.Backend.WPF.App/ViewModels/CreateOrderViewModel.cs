using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Benner.Backend.Application.UseCases.Customer.Queries;
using Benner.Backend.Application.UseCases.Order.Commands;
using Benner.Backend.Application.UseCases.Product.Queries;
using Benner.Backend.Domain.Entities;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;
using Benner.Backend.Shared.Queries;
using Benner.Backend.WPF.App.Commands;
using ICommand = System.Windows.Input.ICommand;

namespace Benner.Backend.WPF.App.ViewModels
{
    public class CreateOrderViewModel : BaseViewModel
    {
        private readonly ICommandHandler<AddOrderItemCommand, Result<Order>> _addOrderItemHandler;
        private readonly ICommandHandler<CreateOrderCommand, Result<Order>> _createOrderHandler;
        private readonly IQueryHandler<GetAllCustomersQuery, Result<IEnumerable<Customer>>> _getAllCustomersHandler;
        private readonly IQueryHandler<GetAllProductsQuery, Result<IEnumerable<Product>>> _getAllProductsHandler;

        private ObservableCollection<Customer> _customers;
        private string _errorMessage;
        private ObservableCollection<Product> _filteredProducts;
        private bool _isLoading;
        private ObservableCollection<OrderItemViewModel> _orderItems;
        private ObservableCollection<Product> _products;
        private string _productSearchText;
        private Customer _selectedCustomer;
        private Product _selectedProduct;
        private int _selectedQuantity = 1;
        private string _successMessage;
        private decimal _totalAmount;

        public CreateOrderViewModel(
            IQueryHandler<GetAllCustomersQuery, Result<IEnumerable<Customer>>> getAllCustomersHandler,
            IQueryHandler<GetAllProductsQuery, Result<IEnumerable<Product>>> getAllProductsHandler,
            ICommandHandler<CreateOrderCommand, Result<Order>> createOrderHandler,
            ICommandHandler<AddOrderItemCommand, Result<Order>> addOrderItemHandler)
        {
            _getAllCustomersHandler = getAllCustomersHandler;
            _getAllProductsHandler = getAllProductsHandler;
            _createOrderHandler = createOrderHandler;
            _addOrderItemHandler = addOrderItemHandler;

            Customers = new ObservableCollection<Customer>();
            Products = new ObservableCollection<Product>();
            FilteredProducts = new ObservableCollection<Product>();
            OrderItems = new ObservableCollection<OrderItemViewModel>();

            LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
            AddItemCommand = new RelayCommand(AddItem, CanAddItem);
            RemoveItemCommand = new RelayCommand<OrderItemViewModel>(RemoveItem);
            CreateOrderCommand = new AsyncRelayCommand(CreateOrderAsync, CanCreateOrder);
            ClearOrderCommand = new RelayCommand(ClearOrder);
            SearchProductsCommand = new RelayCommand(SearchProducts);

            _ = LoadDataAsync();
        }

        #region Properties

        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }

        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                if (SetProperty(ref _selectedCustomer, value))
                {
                    if (CreateOrderCommand is AsyncRelayCommand createCommand)
                        createCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public ObservableCollection<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        public ObservableCollection<Product> FilteredProducts
        {
            get => _filteredProducts;
            set => SetProperty(ref _filteredProducts, value);
        }

        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (SetProperty(ref _selectedProduct, value))
                {
                    if (AddItemCommand is RelayCommand addCommand)
                        addCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string ProductSearchText
        {
            get => _productSearchText;
            set
            {
                if (SetProperty(ref _productSearchText, value))
                {
                    SearchProducts();
                }
            }
        }

        public int SelectedQuantity
        {
            get => _selectedQuantity;
            set
            {
                if (SetProperty(ref _selectedQuantity, value))
                {
                    if (AddItemCommand is RelayCommand addCommand)
                        addCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public ObservableCollection<OrderItemViewModel> OrderItems
        {
            get => _orderItems;
            set => SetProperty(ref _orderItems, value);
        }

        public decimal TotalAmount
        {
            get => _totalAmount;
            set => SetProperty(ref _totalAmount, value);
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

        #endregion

        #region Commands

        public ICommand LoadDataCommand { get; }
        public ICommand AddItemCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand CreateOrderCommand { get; }
        public ICommand ClearOrderCommand { get; }
        public ICommand SearchProductsCommand { get; }

        #endregion

        #region Command Methods

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                ClearMessages();

                var customersTask = _getAllCustomersHandler.HandleAsync(new GetAllCustomersQuery());
                var productsTask = _getAllProductsHandler.HandleAsync(new GetAllProductsQuery());

                await Task.WhenAll(customersTask, productsTask);

                var customersResult = await customersTask;
                var productsResult = await productsTask;

                if (customersResult.IsSuccess && customersResult.Data != null)
                {
                    Customers.Clear();
                    foreach (var customer in customersResult.Data)
                    {
                        Customers.Add(customer);
                    }
                }

                if (productsResult.IsSuccess && productsResult.Data != null)
                {
                    Products.Clear();
                    foreach (var product in productsResult.Data.Where(p => p.StockQuantity > 0))
                    {
                        Products.Add(product);
                    }

                    SearchProducts();
                }

                if (!customersResult.IsSuccess)
                    ErrorMessage = customersResult.Error ?? "Erro ao carregar clientes";
                else if (!productsResult.IsSuccess)
                    ErrorMessage = productsResult.Error ?? "Erro ao carregar produtos";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao carregar dados: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void AddItem()
        {
            if (SelectedProduct == null || SelectedQuantity <= 0) return;

            var existingItem = OrderItems.FirstOrDefault(x => x.ProductId == SelectedProduct.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += SelectedQuantity;
                existingItem.UpdateSubtotal();
            }
            else
            {
                var newItem = new OrderItemViewModel
                {
                    ProductId = SelectedProduct.Id,
                    ProductName = SelectedProduct.Name,
                    UnitPrice = SelectedProduct.Price,
                    Quantity = SelectedQuantity,
                    AvailableStock = SelectedProduct.StockQuantity
                };
                newItem.UpdateSubtotal();
                OrderItems.Add(newItem);
            }

            CalculateTotal();
            SelectedProduct = null;
            SelectedQuantity = 1;

            if (CreateOrderCommand is AsyncRelayCommand createCommand)
                createCommand.RaiseCanExecuteChanged();
        }

        private void RemoveItem(OrderItemViewModel item)
        {
            if (item != null)
            {
                OrderItems.Remove(item);
                CalculateTotal();

                if (CreateOrderCommand is AsyncRelayCommand createCommand)
                    createCommand.RaiseCanExecuteChanged();
            }
        }

        private async Task CreateOrderAsync()
        {
            try
            {
                IsLoading = true;
                ClearMessages();

                var command = new CreateOrderCommand(SelectedCustomer.Id);
                var result = await _createOrderHandler.HandleAsync(command);

                if (result.IsSuccess && result.Data != null)
                {
                    var order = result.Data;

                    foreach (var item in OrderItems)
                    {
                        var addItemCommand = new AddOrderItemCommand(
                            order.Id,
                            item.ProductId,
                            item.Quantity,
                            item.UnitPrice);

                        var addItemResult = await _addOrderItemHandler.HandleAsync(addItemCommand);
                        if (!addItemResult.IsSuccess)
                        {
                            ErrorMessage = $"Erro ao adicionar item: {addItemResult.Error}";
                            return;
                        }
                    }

                    SuccessMessage = $"Pedido #{order.Id} criado com sucesso!";
                    ClearOrder();
                }
                else
                {
                    ErrorMessage = result.Error ?? "Erro ao criar pedido";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao criar pedido: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ClearOrder()
        {
            SelectedCustomer = null;
            OrderItems.Clear();
            CalculateTotal();
            ClearMessages();
        }

        private void SearchProducts()
        {
            FilteredProducts.Clear();

            if (string.IsNullOrWhiteSpace(ProductSearchText))
            {
                foreach (var product in Products)
                {
                    FilteredProducts.Add(product);
                }
            }
            else
            {
                var searchLower = ProductSearchText.ToLower();
                var filtered = Products.Where(p =>
                    p.Name.ToLower().Contains(searchLower) ||
                    p.Code.ToLower().Contains(searchLower) ||
                    p.Category.ToLower().Contains(searchLower));

                foreach (var product in filtered)
                {
                    FilteredProducts.Add(product);
                }
            }
        }

        #endregion

        #region Helper Methods

        private bool CanAddItem()
        {
            return SelectedProduct != null &&
                   SelectedQuantity > 0 &&
                   SelectedQuantity <= SelectedProduct.StockQuantity &&
                   !IsLoading;
        }

        private bool CanCreateOrder()
        {
            return SelectedCustomer != null &&
                   OrderItems.Count > 0 &&
                   !IsLoading;
        }

        private void CalculateTotal()
        {
            TotalAmount = OrderItems.Sum(x => x.Subtotal);
        }

        private void ClearMessages()
        {
            ErrorMessage = null;
            SuccessMessage = null;
        }

        #endregion
    }
}