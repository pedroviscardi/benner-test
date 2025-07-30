using System.Windows;
using Benner.Backend.WPF.App.Services;
using Benner.Backend.WPF.App.ViewModels;
using Benner.Backend.WPF.App.Views;

namespace Benner.Backend.WPF.App
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(MainViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            ShowDashboard();
        }

        private void BtnCustomers_Click(object sender, RoutedEventArgs e)
        {
            ShowCustomers();
        }

        private void BtnProducts_Click(object sender, RoutedEventArgs e)
        {
            ShowProducts();
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            ShowOrders();
        }

        private void BtnCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            ShowCreateOrder();
        }

        private void ShowDashboard()
        {
            DashboardContent.Visibility = Visibility.Visible;
            DynamicContent.Visibility = Visibility.Collapsed;
            DynamicContent.Content = null;
        }

        private void ShowCustomers()
        {
            DashboardContent.Visibility = Visibility.Collapsed;
            DynamicContent.Visibility = Visibility.Visible;

            var customerViewModel = ViewModelFactory.CreateCustomerViewModel();
            var customerView = new CustomerView(customerViewModel);
            DynamicContent.Content = customerView;
        }

        private void ShowProducts()
        {
            DashboardContent.Visibility = Visibility.Collapsed;
            DynamicContent.Visibility = Visibility.Visible;

            var productViewModel = ViewModelFactory.CreateProductViewModel();
            var productView = new ProductView(productViewModel);
            DynamicContent.Content = productView;
        }

        private void ShowOrders()
        {
            DashboardContent.Visibility = Visibility.Collapsed;
            DynamicContent.Visibility = Visibility.Visible;

            var orderViewModel = ViewModelFactory.CreateOrderViewModel();
            var orderView = new OrderView(orderViewModel);
            DynamicContent.Content = orderView;
        }

        private void ShowCreateOrder()
        {
            DashboardContent.Visibility = Visibility.Collapsed;
            DynamicContent.Visibility = Visibility.Visible;

            var createOrderViewModel = ViewModelFactory.CreateCreateOrderViewModel();
            var createOrderView = new CreateOrderView(createOrderViewModel);
            DynamicContent.Content = createOrderView;
        }
    }
}