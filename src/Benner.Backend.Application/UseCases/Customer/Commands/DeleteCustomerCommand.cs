using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;

namespace Benner.Backend.Application.UseCases.Customer.Commands;

public class DeleteCustomerCommand : ICommand<Result<bool>>
{
    public DeleteCustomerCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}