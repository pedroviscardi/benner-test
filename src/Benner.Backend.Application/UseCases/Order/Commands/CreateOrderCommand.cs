using System;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;

namespace Benner.Backend.Application.UseCases.Order.Commands
{
    public class CreateOrderCommand : ICommand<Result<Domain.Entities.Order>>
    {
        public CreateOrderCommand(Guid customerId, string notes = null)
        {
            CustomerId = customerId;
            Notes = notes;
        }

        public Guid CustomerId { get; }
        public string Notes { get; }
    }
}