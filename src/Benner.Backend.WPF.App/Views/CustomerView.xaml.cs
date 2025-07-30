using System.Windows.Controls;
using Benner.Backend.WPF.App.ViewModels;

namespace Benner.Backend.WPF.App.Views
{
    public partial class CustomerView : UserControl
    {
        public CustomerView()
        {
            InitializeComponent();
        }

        public CustomerView(CustomerViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}