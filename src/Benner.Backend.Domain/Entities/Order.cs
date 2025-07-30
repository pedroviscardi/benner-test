using System;
using System.Collections.Generic;
using System.Linq;
using Benner.Backend.Domain.Common;
using Benner.Backend.Domain.Enumerators;

namespace Benner.Backend.Domain.Entities
{
    public class Order : BaseEntity
    {
        public Order()
        {
            Items = new List<OrderItem>();
            DiscountAmount = 0;
        }

        public Order(Guid customerId, List<OrderItem> items = null)
        {
            ValidateOrderData(customerId);

            CustomerId = customerId;
            OrderDate = DateTime.Now;
            Items = items ?? new List<OrderItem>();
            Status = OrderStatus.Pending;
            DiscountAmount = 0;
        }

        private Order(bool skipValidation)
        {
            if (!skipValidation)
                throw new InvalidOperationException("Use o construtor público ou CreateEmpty()");

            CustomerId = Guid.Empty;
            OrderDate = DateTime.Today;
            Items = new List<OrderItem>();
            Status = OrderStatus.Pending;
            DiscountAmount = 0;
        }

        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItem> Items { get; set; }
        public OrderStatus Status { get; set; }
        public decimal DiscountAmount { get; set; }
        public string Notes { get; set; }

        public static Order CreateEmpty()
        {
            return new Order(true);
        }

        public void AddItem(Product product, int quantity, decimal unitPrice)
        {
            var orderItem = new OrderItem(product.Id, quantity, unitPrice);
            Items.Add(orderItem);
        }

        public void AddItem(OrderItem item)
        {
            Items.Add(item);
        }

        public void RemoveItem(Guid productId)
        {
            var itemToRemove = Items.FirstOrDefault(i => i.ProductId == productId);
            if (itemToRemove != null)
            {
                Items.Remove(itemToRemove);
            }
        }

        public void ConfirmOrder()
        {
            Status = OrderStatus.Confirmed;
        }

        public void ApplyDiscount(decimal discountAmount)
        {
            if (discountAmount < 0)
                throw new ArgumentException("Desconto não pode ser negativo", nameof(discountAmount));

            DiscountAmount = discountAmount;
        }

        public void CancelOrder(string notes)
        {
            Status = OrderStatus.Cancelled;
            Notes = notes;
        }

        public void MarkAsDelivered()
        {
            Status = OrderStatus.Delivered;
        }

        private void ValidateOrderData(Guid customerId)
        {
            if (customerId == Guid.Empty)
                throw new ArgumentException("Cliente é obrigatório", nameof(customerId));
        }
    }
}