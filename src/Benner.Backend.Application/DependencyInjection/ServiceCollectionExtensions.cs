using System.Collections.Generic;
using Benner.Backend.Application.UseCases.Customer.Commands;
using Benner.Backend.Application.UseCases.Customer.Handlers;
using Benner.Backend.Application.UseCases.Customer.Queries;
using Benner.Backend.Application.UseCases.Order.Commands;
using Benner.Backend.Application.UseCases.Order.Handlers;
using Benner.Backend.Application.UseCases.Order.Queries;
using Benner.Backend.Application.UseCases.Product.Commands;
using Benner.Backend.Application.UseCases.Product.Handlers;
using Benner.Backend.Application.UseCases.Product.Queries;
using Benner.Backend.Domain.Entities;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;
using Benner.Backend.Shared.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Benner.Backend.Application.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Customer Handlers
            services.AddScoped<ICommandHandler<CreateCustomerCommand, Result<Customer>>, CreateCustomerHandler>();
            services.AddScoped<ICommandHandler<UpdateCustomerCommand, Result<Customer>>, UpdateCustomerHandler>();
            services.AddScoped<ICommandHandler<DeleteCustomerCommand, Result<bool>>, DeleteCustomerHandler>();
            services.AddScoped<IQueryHandler<GetCustomerByIdQuery, Result<Customer>>, GetCustomerByIdHandler>();
            services.AddScoped<IQueryHandler<GetAllCustomersQuery, Result<IEnumerable<Customer>>>, GetAllCustomersHandler>();

            // Product Handlers
            services.AddScoped<ICommandHandler<CreateProductCommand, Result<Product>>, CreateProductHandler>();
            services.AddScoped<ICommandHandler<UpdateProductCommand, Result<Product>>, UpdateProductHandler>();
            services.AddScoped<ICommandHandler<DeleteProductCommand, Result<bool>>, DeleteProductHandler>();
            services.AddScoped<ICommandHandler<UpdateStockCommand, Result<Product>>, UpdateStockHandler>();
            services.AddScoped<IQueryHandler<GetProductByIdQuery, Result<Product>>, GetProductByIdHandler>();
            services.AddScoped<IQueryHandler<GetAllProductsQuery, Result<IEnumerable<Product>>>, GetAllProductsHandler>();
            services.AddScoped<IQueryHandler<GetProductsByCategoryQuery, Result<IEnumerable<Product>>>, GetProductsByCategoryHandler>();
            services.AddScoped<IQueryHandler<GetLowStockProductsQuery, Result<IEnumerable<Product>>>, GetLowStockProductsHandler>();

            // Order Handlers
            services.AddScoped<ICommandHandler<CreateOrderCommand, Result<Order>>, CreateOrderHandler>();
            services.AddScoped<ICommandHandler<AddOrderItemCommand, Result<Order>>, AddOrderItemHandler>();
            services.AddScoped<ICommandHandler<RemoveOrderItemCommand, Result<Order>>, RemoveOrderItemHandler>();
            services.AddScoped<ICommandHandler<ApplyDiscountCommand, Result<Order>>, ApplyDiscountHandler>();
            services.AddScoped<ICommandHandler<ConfirmOrderCommand, Result<Order>>, ConfirmOrderHandler>();
            services.AddScoped<ICommandHandler<CancelOrderCommand, Result<Order>>, CancelOrderHandler>();
            services.AddScoped<ICommandHandler<MarkAsDeliveredCommand, Result<Order>>, MarkAsDeliveredHandler>();
            services.AddScoped<IQueryHandler<GetOrderByIdQuery, Result<Order>>, GetOrderByIdHandler>();
            services.AddScoped<IQueryHandler<GetAllOrdersQuery, Result<IEnumerable<Order>>>, GetAllOrdersHandler>();
            services.AddScoped<IQueryHandler<GetOrdersByCustomerQuery, Result<IEnumerable<Order>>>, GetOrdersByCustomerHandler>();
            services.AddScoped<IQueryHandler<GetOrdersByStatusQuery, Result<IEnumerable<Order>>>, GetOrdersByStatusHandler>();

            return services;
        }
    }
}