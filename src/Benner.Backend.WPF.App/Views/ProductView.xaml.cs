using System.Windows.Controls;
using Benner.Backend.WPF.App.ViewModels;

namespace Benner.Backend.WPF.App.Views
{
    public partial class ProductView : UserControl
    {
        public ProductView()
        {
            InitializeComponent();
        }

        public ProductView(ProductViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}