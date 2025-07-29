using Benner.Backend.Domain.Entities;
using Benner.Backend.Domain.Repositories;

namespace Benner.Backend.Infrastructure.Repositories;

public class OrderRepository : XmlRepository<Order>, IOrderRepository
{
    public OrderRepository(string dataDirectory) : base(dataDirectory)
    {
    }
}