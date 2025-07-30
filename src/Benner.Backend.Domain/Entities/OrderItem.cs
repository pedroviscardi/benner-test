using System;
using Benner.Backend.Domain.Common;

namespace Benner.Backend.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public OrderItem()
        {
        }

        public OrderItem(Guid productId, int quantity, decimal unitPrice)
        {
            ValidateOrderItemData(productId, quantity, unitPrice);

            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        private OrderItem(bool skipValidation)
        {
            if (!skipValidation)
                throw new InvalidOperationException("Use o construtor público ou CreateEmpty()");

            ProductId = Guid.Empty;
            Quantity = 0;
            UnitPrice = 0;
        }

        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;

        public static OrderItem CreateEmpty()
        {
            return new OrderItem(true);
        }

        private void ValidateOrderItemData(Guid productId, int quantity, decimal unitPrice)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("Produto é obrigatório", nameof(productId));

            if (quantity <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantity));

            if (unitPrice < 0)
                throw new ArgumentException("Preço unitário deve ser maior ou igual a zero", nameof(unitPrice));
        }
    }
}