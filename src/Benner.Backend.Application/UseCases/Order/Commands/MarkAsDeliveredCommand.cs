using System;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;

namespace Benner.Backend.Application.UseCases.Order.Commands
{
    public class MarkAsDeliveredCommand : ICommand<Result<Domain.Entities.Order>>
    {
        public MarkAsDeliveredCommand(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; }
    }
}