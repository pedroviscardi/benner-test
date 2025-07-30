using System;
using Benner.Backend.Shared.Common;
using Benner.Backend.Shared.Queries;

namespace Benner.Backend.Application.UseCases.Order.Queries
{
    public class GetOrderByIdQuery : IQuery<Result<Domain.Entities.Order>>
    {
        public GetOrderByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}