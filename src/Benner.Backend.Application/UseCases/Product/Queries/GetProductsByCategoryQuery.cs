using System.Collections.Generic;
using Benner.Backend.Shared.Common;
using Benner.Backend.Shared.Queries;

namespace Benner.Backend.Application.UseCases.Product.Queries
{
    public class GetProductsByCategoryQuery : IQuery<Result<IEnumerable<Domain.Entities.Product>>>
    {
        public GetProductsByCategoryQuery(string category)
        {
            Category = category;
        }

        public string Category { get; }
    }
}