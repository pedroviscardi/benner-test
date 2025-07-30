using System;
using System.Collections.Generic;
using Benner.Backend.Shared.Common;
using Benner.Backend.Shared.Queries;

namespace Benner.Backend.Application.UseCases.Order.Queries
{
    public class GetOrdersByCustomerQuery : IQuery<Result<IEnumerable<Domain.Entities.Order>>>
    {
        public GetOrdersByCustomerQuery(Guid customerId)
        {
            CustomerId = customerId;
        }

        public Guid CustomerId { get; }
    }
}