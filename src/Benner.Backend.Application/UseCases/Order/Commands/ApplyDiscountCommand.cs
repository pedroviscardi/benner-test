using System;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;

namespace Benner.Backend.Application.UseCases.Order.Commands
{
    public class ApplyDiscountCommand : ICommand<Result<Domain.Entities.Order>>
    {
        public ApplyDiscountCommand(Guid orderId, decimal discountAmount)
        {
            OrderId = orderId;
            DiscountAmount = discountAmount;
        }

        public Guid OrderId { get; }
        public decimal DiscountAmount { get; }
    }
}