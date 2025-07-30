using System;
using System.Threading;
using System.Threading.Tasks;
using Benner.Backend.Application.UseCases.Order.Commands;
using Benner.Backend.Domain.Repositories;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;

namespace Benner.Backend.Application.UseCases.Order.Handlers
{
    public class MarkAsDeliveredHandler : ICommandHandler<MarkAsDeliveredCommand, Result<Domain.Entities.Order>>
    {
        private readonly IOrderRepository _orderRepository;

        public MarkAsDeliveredHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task<Result<Domain.Entities.Order>> HandleAsync(MarkAsDeliveredCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                var orderResult = await _orderRepository.GetByIdAsync(command.OrderId);
                if (!orderResult.IsSuccess || orderResult.Data == null)
                    return Result<Domain.Entities.Order>.Failure("Pedido não encontrado");

                var order = orderResult.Data;
                order.MarkAsDelivered();

                var updateResult = await _orderRepository.UpdateAsync(order);
                return updateResult.IsSuccess
                    ? Result<Domain.Entities.Order>.Success(order)
                    : Result<Domain.Entities.Order>.Failure(updateResult.Error);
            }
            catch (Exception ex)
            {
                return Result<Domain.Entities.Order>.Failure($"Erro ao marcar pedido como entregue: {ex.Message}");
            }
        }
    }
}