using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Benner.Backend.Application.UseCases.Customer.Commands;
using Benner.Backend.Domain.Repositories;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;

namespace Benner.Backend.Application.UseCases.Customer.Handlers
{
    public class CreateCustomerHandler : ICommandHandler<CreateCustomerCommand, Result<Domain.Entities.Customer>>
    {
        private readonly ICustomerRepository _customerRepository;

        public CreateCustomerHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        }

        public async Task<Result<Domain.Entities.Customer>> HandleAsync(CreateCustomerCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingCustomers = await _customerRepository.GetAllAsync();
                if (existingCustomers.IsSuccess)
                {
                    if (existingCustomers.Data != null && existingCustomers.Data.Any(existing => existing.Email.Equals(command.Email, StringComparison.OrdinalIgnoreCase)))
                    {
                        return Result<Domain.Entities.Customer>.Failure("Já existe um cliente com este email");
                    }
                }

                var customer = new Domain.Entities.Customer(
                    command.Name,
                    command.Email,
                    command.Phone,
                    command.Document,
                    command.BirthDate,
                    command.Address
                );

                var result = await _customerRepository.AddAsync(customer);
                if (!result.IsSuccess)
                    return Result<Domain.Entities.Customer>.Failure(result.Error);

                return Result<Domain.Entities.Customer>.Success(result.Data);
            }
            catch (Exception ex)
            {
                return Result<Domain.Entities.Customer>.Failure($"Erro ao criar cliente: {ex.Message}");
            }
        }
    }
}