using System;
using System.Threading;
using System.Threading.Tasks;
using Benner.Backend.Application.UseCases.Order.Commands;
using Benner.Backend.Domain.Repositories;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;

namespace Benner.Backend.Application.UseCases.Order.Handlers
{
    public class CreateOrderHandler : ICommandHandler<CreateOrderCommand, Result<Domain.Entities.Order>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;

        public CreateOrderHandler(IOrderRepository orderRepository, ICustomerRepository customerRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        }

        public async Task<Result<Domain.Entities.Order>> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                var customerExists = await _customerRepository.ExistsAsync(command.CustomerId);
                if (!customerExists.IsSuccess || !customerExists.Data)
                    return Result<Domain.Entities.Order>.Failure("Cliente não encontrado");

                var order = new Domain.Entities.Order(command.CustomerId);
                var result = await _orderRepository.AddAsync(order);

                return result.IsSuccess
                    ? Result<Domain.Entities.Order>.Success(order)
                    : Result<Domain.Entities.Order>.Failure(result.Error);
            }
            catch (Exception ex)
            {
                return Result<Domain.Entities.Order>.Failure($"Erro ao criar pedido: {ex.Message}");
            }
        }
    }
}