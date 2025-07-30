using Benner.Backend.Domain.Entities;
using Benner.Backend.Domain.Repositories;

namespace Benner.Backend.Infrastructure.Repositories
{
    public class ProductRepository : XmlRepository<Product>, IProductRepository
    {
        public ProductRepository(string dataDirectory) : base(dataDirectory)
        {
        }
    }
}