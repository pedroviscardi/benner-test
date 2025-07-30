using System;
using System.Threading;
using System.Threading.Tasks;
using Benner.Backend.Application.UseCases.Product.Commands;
using Benner.Backend.Domain.Repositories;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;

namespace Benner.Backend.Application.UseCases.Product.Handlers
{
    public class UpdateStockHandler : ICommandHandler<UpdateStockCommand, Result<Domain.Entities.Product>>
    {
        private readonly IProductRepository _productRepository;

        public UpdateStockHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        public async Task<Result<Domain.Entities.Product>> HandleAsync(UpdateStockCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                var productResult = await _productRepository.GetByIdAsync(command.Id);
                if (!productResult.IsSuccess || productResult.Data == null)
                    return Result<Domain.Entities.Product>.Failure("Produto não encontrado");

                var product = productResult.Data;
                product.UpdateStock(command.NewQuantity);

                var updateResult = await _productRepository.UpdateAsync(product);
                return updateResult.IsSuccess
                    ? Result<Domain.Entities.Product>.Success(product)
                    : Result<Domain.Entities.Product>.Failure(updateResult.Error);
            }
            catch (Exception ex)
            {
                return Result<Domain.Entities.Product>.Failure($"Erro ao atualizar estoque: {ex.Message}");
            }
        }
    }
}