using Benner.Backend.Domain.Entities;
using Benner.Backend.Domain.Repositories;

namespace Benner.Backend.Infrastructure.Repositories;

public class CustomerRepository : XmlRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(string dataDirectory) : base(dataDirectory)
    {
    }
}