using System;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;

namespace Benner.Backend.Application.UseCases.Order.Commands
{
    public class AddOrderItemCommand : ICommand<Result<Domain.Entities.Order>>
    {
        public AddOrderItemCommand(Guid orderId, Guid productId, int quantity, decimal unitPrice)
        {
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        public Guid OrderId { get; }
        public Guid ProductId { get; }
        public int Quantity { get; }
        public decimal UnitPrice { get; }
    }
}