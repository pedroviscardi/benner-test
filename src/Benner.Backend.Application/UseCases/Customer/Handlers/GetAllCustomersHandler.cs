using Benner.Backend.Application.UseCases.Customer.Queries;
using Benner.Backend.Domain.Repositories;
using Benner.Backend.Shared.Common;
using Benner.Backend.Shared.Queries;

namespace Benner.Backend.Application.UseCases.Customer.Handlers;

public class GetAllCustomersHandler : IQueryHandler<GetAllCustomersQuery, Result<IEnumerable<Domain.Entities.Customer>>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetAllCustomersHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
    }

    public async Task<Result<IEnumerable<Domain.Entities.Customer>>> HandleAsync(GetAllCustomersQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _customerRepository.GetAllAsync();
            return result;
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<Domain.Entities.Customer>>.Failure($"Erro ao buscar clientes: {ex.Message}");
        }
    }
}