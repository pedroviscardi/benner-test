using System.Collections.Generic;
using Benner.Backend.Application.UseCases.Customer.Commands;
using Benner.Backend.Application.UseCases.Customer.Queries;
using Benner.Backend.Application.UseCases.Order.Commands;
using Benner.Backend.Application.UseCases.Order.Queries;
using Benner.Backend.Application.UseCases.Product.Commands;
using Benner.Backend.Application.UseCases.Product.Queries;
using Benner.Backend.Domain.Entities;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;
using Benner.Backend.Shared.Queries;
using Benner.Backend.WPF.App.ViewModels;

namespace Benner.Backend.WPF.App.Services
{
    public static class ViewModelFactory
    {
        public static MainViewModel CreateMainViewModel()
        {
            return new MainViewModel(
                ServiceLocator.GetService<IQueryHandler<GetAllCustomersQuery, Result<IEnumerable<Customer>>>>(),
                ServiceLocator.GetService<IQueryHandler<GetAllProductsQuery, Result<IEnumerable<Product>>>>(),
                ServiceLocator.GetService<IQueryHandler<GetAllOrdersQuery, Result<IEnumerable<Order>>>>(),
                ServiceLocator.GetService<IQueryHandler<GetLowStockProductsQuery, Result<IEnumerable<Product>>>>()
            );
        }

        public static CustomerViewModel CreateCustomerViewModel()
        {
            return new CustomerViewModel(
                ServiceLocator.GetService<IQueryHandler<GetAllCustomersQuery, Result<IEnumerable<Customer>>>>(),
                ServiceLocator.GetService<ICommandHandler<CreateCustomerCommand, Result<Customer>>>(),
                ServiceLocator.GetService<ICommandHandler<UpdateCustomerCommand, Result<Customer>>>(),
                ServiceLocator.GetService<ICommandHandler<DeleteCustomerCommand, Result<bool>>>()
            );
        }

        public static ProductViewModel CreateProductViewModel()
        {
            return new ProductViewModel(
                ServiceLocator.GetService<IQueryHandler<GetAllProductsQuery, Result<IEnumerable<Product>>>>(),
                ServiceLocator.GetService<ICommandHandler<CreateProductCommand, Result<Product>>>(),
                ServiceLocator.GetService<ICommandHandler<UpdateProductCommand, Result<Product>>>(),
                ServiceLocator.GetService<ICommandHandler<DeleteProductCommand, Result<bool>>>(),
                ServiceLocator.GetService<ICommandHandler<UpdateStockCommand, Result<Product>>>()
            );
        }

        public static OrderViewModel CreateOrderViewModel()
        {
            return new OrderViewModel(
                ServiceLocator.GetService<IQueryHandler<GetAllOrdersQuery, Result<IEnumerable<Order>>>>(),
                ServiceLocator.GetService<ICommandHandler<CreateOrderCommand, Result<Order>>>(),
                ServiceLocator.GetService<ICommandHandler<AddOrderItemCommand, Result<Order>>>(),
                ServiceLocator.GetService<ICommandHandler<RemoveOrderItemCommand, Result<Order>>>(),
                ServiceLocator.GetService<ICommandHandler<ConfirmOrderCommand, Result<Order>>>(),
                ServiceLocator.GetService<ICommandHandler<CancelOrderCommand, Result<Order>>>(),
                ServiceLocator.GetService<ICommandHandler<MarkAsDeliveredCommand, Result<Order>>>()
            );
        }

        public static CreateOrderViewModel CreateCreateOrderViewModel()
        {
            return new CreateOrderViewModel(
                ServiceLocator.GetService<IQueryHandler<GetAllCustomersQuery, Result<IEnumerable<Customer>>>>(),
                ServiceLocator.GetService<IQueryHandler<GetAllProductsQuery, Result<IEnumerable<Product>>>>(),
                ServiceLocator.GetService<ICommandHandler<CreateOrderCommand, Result<Order>>>(),
                ServiceLocator.GetService<ICommandHandler<AddOrderItemCommand, Result<Order>>>()
            );
        }
    }
}