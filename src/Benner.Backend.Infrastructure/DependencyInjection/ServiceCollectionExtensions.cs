using Benner.Backend.Domain.Repositories;
using Benner.Backend.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Benner.Backend.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string dataDirectory)
        {
            services.AddSingleton<ICustomerRepository>(provider => new CustomerRepository(dataDirectory));
            services.AddSingleton<IProductRepository>(provider => new ProductRepository(dataDirectory));
            services.AddSingleton<IOrderRepository>(provider => new OrderRepository(dataDirectory));

            return services;
        }
    }
}