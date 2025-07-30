using System;
using System.Threading;
using System.Threading.Tasks;
using Benner.Backend.Application.UseCases.Product.Queries;
using Benner.Backend.Domain.Repositories;
using Benner.Backend.Shared.Common;
using Benner.Backend.Shared.Queries;

namespace Benner.Backend.Application.UseCases.Product.Handlers
{
    public class GetProductByIdHandler : IQueryHandler<GetProductByIdQuery, Result<Domain.Entities.Product>>
    {
        private readonly IProductRepository _productRepository;

        public GetProductByIdHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        public async Task<Result<Domain.Entities.Product>> HandleAsync(GetProductByIdQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _productRepository.GetByIdAsync(query.Id);
                return result;
            }
            catch (Exception ex)
            {
                return Result<Domain.Entities.Product>.Failure($"Erro ao buscar produto: {ex.Message}");
            }
        }
    }
}