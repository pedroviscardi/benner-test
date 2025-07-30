using System;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;

namespace Benner.Backend.Application.UseCases.Order.Commands
{
    public class ConfirmOrderCommand : ICommand<Result<Domain.Entities.Order>>
    {
        public ConfirmOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; }
    }
}