using System;
using System.Threading;
using System.Threading.Tasks;
using Benner.Backend.Application.UseCases.Customer.Commands;
using Benner.Backend.Domain.Repositories;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;

namespace Benner.Backend.Application.UseCases.Customer.Handlers
{
    public class UpdateCustomerHandler : ICommandHandler<UpdateCustomerCommand, Result<Domain.Entities.Customer>>
    {
        private readonly ICustomerRepository _customerRepository;

        public UpdateCustomerHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        }

        public async Task<Result<Domain.Entities.Customer>> HandleAsync(UpdateCustomerCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingResult = await _customerRepository.GetByIdAsync(command.Id);
                if (!existingResult.IsSuccess)
                    return Result<Domain.Entities.Customer>.Failure("Cliente não encontrado");

                var customer = existingResult.Data;

                customer.UpdatePersonalInfo(command.Name, command.Email, command.Phone, command.BirthDate);
                customer.UpdateAddress(command.Address);

                var updateResult = await _customerRepository.UpdateAsync(customer);
                if (!updateResult.IsSuccess)
                    return Result<Domain.Entities.Customer>.Failure(updateResult.Error);

                return Result<Domain.Entities.Customer>.Success(updateResult.Data);
            }
            catch (Exception ex)
            {
                return Result<Domain.Entities.Customer>.Failure($"Erro ao atualizar cliente: {ex.Message}");
            }
        }
    }
}