using System;
using System.Collections.Generic;
using System.IO;
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
using Benner.Backend.Infrastructure.Repositories;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;
using Benner.Backend.Shared.Queries;

namespace Benner.Backend.WPF.App.Services
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
        private static bool _initialized;

        public static void Initialize()
        {
            if (_initialized) return;

            var dataDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BennerApp",
                "Data");
            Directory.CreateDirectory(dataDirectory);

            // Repositories
            var customerRepository = new CustomerRepository(dataDirectory);
            var productRepository = new ProductRepository(dataDirectory);
            var orderRepository = new OrderRepository(dataDirectory);

            // Register Query Handlers
            _services[typeof(IQueryHandler<GetAllCustomersQuery, Result<IEnumerable<Customer>>>)] =
                new GetAllCustomersHandler(customerRepository);

            _services[typeof(IQueryHandler<GetAllProductsQuery, Result<IEnumerable<Product>>>)] =
                new GetAllProductsHandler(productRepository);

            _services[typeof(IQueryHandler<GetAllOrdersQuery, Result<IEnumerable<Order>>>)] =
                new GetAllOrdersHandler(orderRepository);

            _services[typeof(IQueryHandler<GetLowStockProductsQuery, Result<IEnumerable<Product>>>)] =
                new GetLowStockProductsHandler(productRepository);

            // Register Command Handlers
            _services[typeof(ICommandHandler<CreateCustomerCommand, Result<Customer>>)] =
                new CreateCustomerHandler(customerRepository);

            _services[typeof(ICommandHandler<UpdateCustomerCommand, Result<Customer>>)] =
                new UpdateCustomerHandler(customerRepository);

            _services[typeof(ICommandHandler<DeleteCustomerCommand, Result<bool>>)] =
                new DeleteCustomerHandler(customerRepository);

            _services[typeof(ICommandHandler<CreateProductCommand, Result<Product>>)] =
                new CreateProductHandler(productRepository);

            _services[typeof(ICommandHandler<UpdateProductCommand, Result<Product>>)] =
                new UpdateProductHandler(productRepository);

            _services[typeof(ICommandHandler<DeleteProductCommand, Result<bool>>)] =
                new DeleteProductHandler(productRepository);

            _services[typeof(ICommandHandler<UpdateStockCommand, Result<Product>>)] =
                new UpdateStockHandler(productRepository);

            _services[typeof(ICommandHandler<CreateOrderCommand, Result<Order>>)] =
                new CreateOrderHandler(orderRepository, customerRepository);

            _services[typeof(ICommandHandler<AddOrderItemCommand, Result<Order>>)] =
                new AddOrderItemHandler(orderRepository, productRepository);

            _services[typeof(ICommandHandler<RemoveOrderItemCommand, Result<Order>>)] =
                new RemoveOrderItemHandler(orderRepository);

            _services[typeof(ICommandHandler<ConfirmOrderCommand, Result<Order>>)] =
                new ConfirmOrderHandler(orderRepository);

            _services[typeof(ICommandHandler<CancelOrderCommand, Result<Order>>)] =
                new CancelOrderHandler(orderRepository);

            _services[typeof(ICommandHandler<MarkAsDeliveredCommand, Result<Order>>)] =
                new MarkAsDeliveredHandler(orderRepository);

            _services[typeof(ICommandHandler<ApplyDiscountCommand, Result<Order>>)] =
                new ApplyDiscountHandler(orderRepository);

            _initialized = true;
        }

        public static T GetService<T>()
        {
            if (!_initialized)
                throw new InvalidOperationException("ServiceLocator not initialized");

            if (_services.TryGetValue(typeof(T), out var service))
                return (T) service;

            throw new InvalidOperationException($"Service {typeof(T).Name} not registered");
        }
    }
}