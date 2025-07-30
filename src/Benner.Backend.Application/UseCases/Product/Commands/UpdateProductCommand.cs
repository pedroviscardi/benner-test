using System;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;

namespace Benner.Backend.Application.UseCases.Product.Commands
{
    public class UpdateProductCommand : ICommand<Result<Domain.Entities.Product>>
    {
        public UpdateProductCommand(Guid id,
            string name,
            string description,
            decimal price,
            int stockQuantity,
            int minimumStock,
            string category,
            string brand)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            StockQuantity = stockQuantity;
            MinimumStock = minimumStock;
            Category = category;
            Brand = brand;
        }

        public Guid Id { get; }
        public string Name { get; }
        public string Description { get; }
        public decimal Price { get; }
        public int StockQuantity { get; }
        public int MinimumStock { get; }
        public string Category { get; }
        public string Brand { get; }
    }
}