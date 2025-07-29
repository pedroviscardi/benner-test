using Benner.Backend.Application.UseCases.Customer.Commands;
using Benner.Backend.Domain.Repositories;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;

namespace Benner.Backend.Application.UseCases.Customer.Handlers;

public class DeleteCustomerHandler : ICommandHandler<DeleteCustomerCommand, Result<bool>>
{
    private readonly ICustomerRepository _customerRepository;

    public DeleteCustomerHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
    }

    public async Task<Result<bool>> HandleAsync(DeleteCustomerCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var existsResult = await _customerRepository.ExistsAsync(command.Id);
            if (!existsResult.IsSuccess || !existsResult.Data)
                return Result<bool>.Failure("Cliente não encontrado");

            var deleteResult = await _customerRepository.DeleteAsync(command.Id);
            if (!deleteResult.IsSuccess)
                return Result<bool>.Failure(deleteResult.Error);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Erro ao excluir cliente: {ex.Message}");
        }
    }
}