using System;
using System.Threading;
using System.Threading.Tasks;
using Benner.Backend.Application.UseCases.Order.Commands;
using Benner.Backend.Domain.Repositories;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;

namespace Benner.Backend.Application.UseCases.Order.Handlers
{
    public class AddOrderItemHandler : ICommandHandler<AddOrderItemCommand, Result<Domain.Entities.Order>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public AddOrderItemHandler(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        public async Task<Result<Domain.Entities.Order>> HandleAsync(AddOrderItemCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                var orderResult = await _orderRepository.GetByIdAsync(command.OrderId);
                if (!orderResult.IsSuccess || orderResult.Data == null)
                    return Result<Domain.Entities.Order>.Failure("Pedido não encontrado");

                var productResult = await _productRepository.GetByIdAsync(command.ProductId);
                if (!productResult.IsSuccess || productResult.Data == null)
                    return Result<Domain.Entities.Order>.Failure("Produto não encontrado");

                var order = orderResult.Data;
                var product = productResult.Data;

                order.AddItem(product, command.Quantity, command.UnitPrice);

                var updateResult = await _orderRepository.UpdateAsync(order);
                return updateResult.IsSuccess
                    ? Result<Domain.Entities.Order>.Success(order)
                    : Result<Domain.Entities.Order>.Failure(updateResult.Error);
            }
            catch (Exception ex)
            {
                return Result<Domain.Entities.Order>.Failure($"Erro ao adicionar item ao pedido: {ex.Message}");
            }
        }
    }
}