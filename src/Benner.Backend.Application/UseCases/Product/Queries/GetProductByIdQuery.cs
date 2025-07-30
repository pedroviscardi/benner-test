using System;
using Benner.Backend.Shared.Common;
using Benner.Backend.Shared.Queries;

namespace Benner.Backend.Application.UseCases.Product.Queries
{
    public class GetProductByIdQuery : IQuery<Result<Domain.Entities.Product>>
    {
        public GetProductByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}