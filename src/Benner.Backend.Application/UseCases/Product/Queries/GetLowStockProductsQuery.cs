using System.Collections.Generic;
using Benner.Backend.Shared.Common;
using Benner.Backend.Shared.Queries;

namespace Benner.Backend.Application.UseCases.Product.Queries
{
    public class GetLowStockProductsQuery : IQuery<Result<IEnumerable<Domain.Entities.Product>>>
    {
    }
}