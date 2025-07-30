using System;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;

namespace Benner.Backend.Application.UseCases.Order.Commands
{
    public class CancelOrderCommand : ICommand<Result<Domain.Entities.Order>>
    {
        public CancelOrderCommand(Guid orderId, string reason)
        {
            OrderId = orderId;
            Reason = reason;
        }

        public Guid OrderId { get; }
        public string Reason { get; }
    }
}