using System.Windows.Controls;
using Benner.Backend.WPF.App.ViewModels;

namespace Benner.Backend.WPF.App.Views
{
    public partial class CreateOrderView : UserControl
    {
        public CreateOrderView()
        {
            InitializeComponent();
        }

        public CreateOrderView(CreateOrderViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}