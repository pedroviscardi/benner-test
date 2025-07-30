using System.Windows.Controls;
using Benner.Backend.WPF.App.ViewModels;

namespace Benner.Backend.WPF.App.Views
{
    public partial class OrderView : UserControl
    {
        public OrderView()
        {
            InitializeComponent();
        }

        public OrderView(OrderViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}