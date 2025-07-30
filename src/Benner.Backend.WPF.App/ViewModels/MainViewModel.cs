using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Benner.Backend.Application.UseCases.Customer.Queries;
using Benner.Backend.Application.UseCases.Order.Queries;
using Benner.Backend.Application.UseCases.Product.Queries;
using Benner.Backend.Domain.Entities;
using Benner.Backend.Shared.Common;
using Benner.Backend.Shared.Queries;
using Benner.Backend.WPF.App.Commands;

namespace Benner.Backend.WPF.App.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IQueryHandler<GetAllCustomersQuery, Result<IEnumerable<Customer>>> _getAllCustomersHandler;
        private readonly IQueryHandler<GetAllOrdersQuery, Result<IEnumerable<Order>>> _getAllOrdersHandler;
        private readonly IQueryHandler<GetAllProductsQuery, Result<IEnumerable<Product>>> _getAllProductsHandler;
        private readonly IQueryHandler<GetLowStockProductsQuery, Result<IEnumerable<Product>>> _getLowStockProductsHandler;
        private bool _isLoading;
        private int _lowStockProducts;
        private string _statusMessage;

        private int _totalCustomers;
        private int _totalOrders;
        private int _totalProducts;

        public MainViewModel(
            IQueryHandler<GetAllCustomersQuery, Result<IEnumerable<Customer>>> getAllCustomersHandler,
            IQueryHandler<GetAllProductsQuery, Result<IEnumerable<Product>>> getAllProductsHandler,
            IQueryHandler<GetAllOrdersQuery, Result<IEnumerable<Order>>> getAllOrdersHandler,
            IQueryHandler<GetLowStockProductsQuery, Result<IEnumerable<Product>>> getLowStockProductsHandler)
        {
            _getAllCustomersHandler = getAllCustomersHandler;
            _getAllProductsHandler = getAllProductsHandler;
            _getAllOrdersHandler = getAllOrdersHandler;
            _getLowStockProductsHandler = getLowStockProductsHandler;

            LoadDashboardCommand = new AsyncRelayCommand(LoadDashboardAsync);
            RefreshCommand = new AsyncRelayCommand(LoadDashboardAsync);

            _ = LoadDashboardAsync();
        }

        public int TotalCustomers
        {
            get => _totalCustomers;
            set => SetProperty(ref _totalCustomers, value);
        }

        public int TotalProducts
        {
            get => _totalProducts;
            set => SetProperty(ref _totalProducts, value);
        }

        public int TotalOrders
        {
            get => _totalOrders;
            set => SetProperty(ref _totalOrders, value);
        }

        public int LowStockProducts
        {
            get => _lowStockProducts;
            set => SetProperty(ref _lowStockProducts, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ICommand LoadDashboardCommand { get; }
        public ICommand RefreshCommand { get; }

        private async Task LoadDashboardAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Carregando dados do dashboard...";

                var customersResult = await _getAllCustomersHandler.HandleAsync(new GetAllCustomersQuery());
                TotalCustomers = customersResult.IsSuccess ? customersResult.Data?.Count() ?? 0 : 0;

                var productsResult = await _getAllProductsHandler.HandleAsync(new GetAllProductsQuery());
                TotalProducts = productsResult.IsSuccess ? productsResult.Data?.Count() ?? 0 : 0;

                var ordersResult = await _getAllOrdersHandler.HandleAsync(new GetAllOrdersQuery());
                TotalOrders = ordersResult.IsSuccess ? ordersResult.Data?.Count() ?? 0 : 0;

                var lowStockResult = await _getLowStockProductsHandler.HandleAsync(new GetLowStockProductsQuery());
                LowStockProducts = lowStockResult.IsSuccess ? lowStockResult.Data?.Count() ?? 0 : 0;

                StatusMessage = "Dashboard carregado com sucesso";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro ao carregar dashboard: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}