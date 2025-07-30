using System.Collections.Generic;
using Benner.Backend.Domain.Enumerators;
using Benner.Backend.Shared.Common;
using Benner.Backend.Shared.Queries;

namespace Benner.Backend.Application.UseCases.Order.Queries
{
    public class GetOrdersByStatusQuery : IQuery<Result<IEnumerable<Domain.Entities.Order>>>
    {
        public GetOrdersByStatusQuery(OrderStatus status)
        {
            Status = status;
        }

        public OrderStatus Status { get; }
    }
}