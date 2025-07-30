using System;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;

namespace Benner.Backend.Application.UseCases.Order.Commands
{
    public class RemoveOrderItemCommand : ICommand<Result<Domain.Entities.Order>>
    {
        public RemoveOrderItemCommand(Guid orderId, Guid productId)
        {
            OrderId = orderId;
            ProductId = productId;
        }

        public Guid OrderId { get; }
        public Guid ProductId { get; }
    }
}