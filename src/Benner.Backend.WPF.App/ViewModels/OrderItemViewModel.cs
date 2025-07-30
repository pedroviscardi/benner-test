using System;

namespace Benner.Backend.WPF.App.ViewModels
{
    public class OrderItemViewModel : BaseViewModel
    {
        private int _quantity;
        private decimal _subtotal;

        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int AvailableStock { get; set; }

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (SetProperty(ref _quantity, value))
                {
                    UpdateSubtotal();
                }
            }
        }

        public decimal Subtotal
        {
            get => _subtotal;
            private set => SetProperty(ref _subtotal, value);
        }

        public void UpdateSubtotal()
        {
            Subtotal = UnitPrice * Quantity;
        }
    }
}