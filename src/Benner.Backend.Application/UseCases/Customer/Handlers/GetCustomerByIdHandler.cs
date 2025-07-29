using Benner.Backend.Application.UseCases.Customer.Queries;
using Benner.Backend.Domain.Repositories;
using Benner.Backend.Shared.Common;
using Benner.Backend.Shared.Queries;

namespace Benner.Backend.Application.UseCases.Customer.Handlers;

public class GetCustomerByIdHandler : IQueryHandler<GetCustomerByIdQuery, Result<Domain.Entities.Customer>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByIdHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
    }

    public async Task<Result<Domain.Entities.Customer>> HandleAsync(GetCustomerByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _customerRepository.GetByIdAsync(query.Id);
            return result;
        }
        catch (Exception ex)
        {
            return Result<Domain.Entities.Customer>.Failure($"Erro ao buscar cliente: {ex.Message}");
        }
    }
}