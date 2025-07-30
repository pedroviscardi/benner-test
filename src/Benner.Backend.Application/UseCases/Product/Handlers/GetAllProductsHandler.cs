using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Benner.Backend.Application.UseCases.Product.Queries;
using Benner.Backend.Domain.Repositories;
using Benner.Backend.Shared.Common;
using Benner.Backend.Shared.Queries;

namespace Benner.Backend.Application.UseCases.Product.Handlers
{
    public class GetAllProductsHandler : IQueryHandler<GetAllProductsQuery, Result<IEnumerable<Domain.Entities.Product>>>
    {
        private readonly IProductRepository _productRepository;

        public GetAllProductsHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        public async Task<Result<IEnumerable<Domain.Entities.Product>>> HandleAsync(GetAllProductsQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _productRepository.GetAllAsync();
                return result.IsSuccess
                    ? Result<IEnumerable<Domain.Entities.Product>>.Success(result.Data ?? Enumerable.Empty<Domain.Entities.Product>())
                    : Result<IEnumerable<Domain.Entities.Product>>.Failure(result.Error);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Domain.Entities.Product>>.Failure($"Erro ao listar produtos: {ex.Message}");
            }
        }
    }
}