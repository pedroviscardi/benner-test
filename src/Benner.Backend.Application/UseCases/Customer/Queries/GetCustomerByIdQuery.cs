using Benner.Backend.Shared.Common;
using Benner.Backend.Shared.Queries;

namespace Benner.Backend.Application.UseCases.Customer.Queries;

public class GetCustomerByIdQuery : IQuery<Result<Domain.Entities.Customer>>
{
    public GetCustomerByIdQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}