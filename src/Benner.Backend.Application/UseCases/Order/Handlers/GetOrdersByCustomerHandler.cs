using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Benner.Backend.Application.UseCases.Order.Queries;
using Benner.Backend.Domain.Repositories;
using Benner.Backend.Shared.Common;
using Benner.Backend.Shared.Queries;

namespace Benner.Backend.Application.UseCases.Order.Handlers
{
    public class GetOrdersByCustomerHandler : IQueryHandler<GetOrdersByCustomerQuery, Result<IEnumerable<Domain.Entities.Order>>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrdersByCustomerHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task<Result<IEnumerable<Domain.Entities.Order>>> HandleAsync(GetOrdersByCustomerQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _orderRepository.FindAsync(o => o.CustomerId == query.CustomerId);
                return result.IsSuccess
                    ? Result<IEnumerable<Domain.Entities.Order>>.Success(result.Data ?? Enumerable.Empty<Domain.Entities.Order>())
                    : Result<IEnumerable<Domain.Entities.Order>>.Failure(result.Error);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Domain.Entities.Order>>.Failure($"Erro ao buscar pedidos do cliente: {ex.Message}");
            }
        }
    }
}