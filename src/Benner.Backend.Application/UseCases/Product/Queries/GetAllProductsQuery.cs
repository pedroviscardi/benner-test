using System.Collections.Generic;
using Benner.Backend.Shared.Common;
using Benner.Backend.Shared.Queries;

namespace Benner.Backend.Application.UseCases.Product.Queries
{
    public class GetAllProductsQuery : IQuery<Result<IEnumerable<Domain.Entities.Product>>>
    {
    }
}