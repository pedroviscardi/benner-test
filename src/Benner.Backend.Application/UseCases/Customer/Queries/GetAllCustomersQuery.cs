using Benner.Backend.Shared.Common;
using Benner.Backend.Shared.Queries;

namespace Benner.Backend.Application.UseCases.Customer.Queries;

public class GetAllCustomersQuery : IQuery<Result<IEnumerable<Domain.Entities.Customer>>>
{
}