using System.Collections.Generic;
using Benner.Backend.Shared.Common;
using Benner.Backend.Shared.Queries;

namespace Benner.Backend.Application.UseCases.Order.Queries
{
    public class GetAllOrdersQuery : IQuery<Result<IEnumerable<Domain.Entities.Order>>>
    {
    }
}