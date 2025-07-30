using System;
using System.Threading;
using System.Threading.Tasks;
using Benner.Backend.Application.UseCases.Order.Queries;
using Benner.Backend.Domain.Repositories;
using Benner.Backend.Shared.Common;
using Benner.Backend.Shared.Queries;

namespace Benner.Backend.Application.UseCases.Order.Handlers
{
    public class GetOrderByIdHandler : IQueryHandler<GetOrderByIdQuery, Result<Domain.Entities.Order>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderByIdHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task<Result<Domain.Entities.Order>> HandleAsync(GetOrderByIdQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _orderRepository.GetByIdAsync(query.Id);
                return result;
            }
            catch (Exception ex)
            {
                return Result<Domain.Entities.Order>.Failure($"Erro ao buscar pedido: {ex.Message}");
            }
        }
    }
}