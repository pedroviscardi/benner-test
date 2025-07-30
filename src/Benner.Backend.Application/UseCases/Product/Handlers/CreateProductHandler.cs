using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Benner.Backend.Application.UseCases.Product.Commands;
using Benner.Backend.Domain.Repositories;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;

namespace Benner.Backend.Application.UseCases.Product.Handlers
{
    public class CreateProductHandler : ICommandHandler<CreateProductCommand, Result<Domain.Entities.Product>>
    {
        private readonly IProductRepository _productRepository;

        public CreateProductHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        public async Task<Result<Domain.Entities.Product>> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingProducts = await _productRepository.GetAllAsync();
                if (existingProducts.IsSuccess && existingProducts.Data != null)
                {
                    var codeExists = existingProducts.Data.Any(p => p.Code.Equals(command.Code, StringComparison.OrdinalIgnoreCase));
                    if (codeExists)
                        return Result<Domain.Entities.Product>.Failure("Já existe um produto com este código");
                }

                var product = new Domain.Entities.Product(command.Name, command.Description, command.Code,
                    command.Price, command.StockQuantity, command.MinimumStock, command.Category, command.Brand);

                var result = await _productRepository.AddAsync(product);

                return result.IsSuccess
                    ? Result<Domain.Entities.Product>.Success(product)
                    : Result<Domain.Entities.Product>.Failure(result.Error);
            }
            catch (Exception ex)
            {
                return Result<Domain.Entities.Product>.Failure($"Erro ao criar produto: {ex.Message}");
            }
        }
    }
}