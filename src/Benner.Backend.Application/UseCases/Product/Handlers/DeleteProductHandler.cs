using System;
using System.Threading;
using System.Threading.Tasks;
using Benner.Backend.Application.UseCases.Product.Commands;
using Benner.Backend.Domain.Repositories;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;

namespace Benner.Backend.Application.UseCases.Product.Handlers
{
    public class DeleteProductHandler : ICommandHandler<DeleteProductCommand, Result<bool>>
    {
        private readonly IProductRepository _productRepository;

        public DeleteProductHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        public async Task<Result<bool>> HandleAsync(DeleteProductCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                var existsResult = await _productRepository.ExistsAsync(command.Id);
                if (!existsResult.IsSuccess || !existsResult.Data)
                    return Result<bool>.Failure("Produto não encontrado");

                var deleteResult = await _productRepository.DeleteAsync(command.Id);
                if (!deleteResult.IsSuccess)
                    return Result<bool>.Failure(deleteResult.Error);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Erro ao excluir produto: {ex.Message}");
            }
        }
    }
}