using System;
using System.Threading;
using System.Threading.Tasks;
using Benner.Backend.Application.UseCases.Order.Commands;
using Benner.Backend.Domain.Repositories;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;

namespace Benner.Backend.Application.UseCases.Order.Handlers
{
    public class RemoveOrderItemHandler : ICommandHandler<RemoveOrderItemCommand, Result<Domain.Entities.Order>>
    {
        private readonly IOrderRepository _orderRepository;

        public RemoveOrderItemHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task<Result<Domain.Entities.Order>> HandleAsync(RemoveOrderItemCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                var orderResult = await _orderRepository.GetByIdAsync(command.OrderId);
                if (!orderResult.IsSuccess || orderResult.Data == null)
                    return Result<Domain.Entities.Order>.Failure("Pedido não encontrado");

                var order = orderResult.Data;
                order.RemoveItem(command.ProductId);

                var updateResult = await _orderRepository.UpdateAsync(order);
                return updateResult.IsSuccess
                    ? Result<Domain.Entities.Order>.Success(order)
                    : Result<Domain.Entities.Order>.Failure(updateResult.Error);
            }
            catch (Exception ex)
            {
                return Result<Domain.Entities.Order>.Failure($"Erro ao remover item do pedido: {ex.Message}");
            }
        }
    }
}