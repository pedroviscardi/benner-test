using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;

namespace Benner.Backend.Application.UseCases.Product.Commands
{
    public class CreateProductCommand : ICommand<Result<Domain.Entities.Product>>
    {
        public CreateProductCommand(string name,
            string description,
            string code,
            decimal price,
            int stockQuantity,
            int minimumStock,
            string category,
            string brand)
        {
            Name = name;
            Description = description;
            Code = code;
            Price = price;
            StockQuantity = stockQuantity;
            MinimumStock = minimumStock;
            Category = category;
            Brand = brand;
        }

        public string Name { get; }
        public string Description { get; }
        public string Code { get; }
        public decimal Price { get; }
        public int StockQuantity { get; }
        public int MinimumStock { get; }
        public string Category { get; }
        public string Brand { get; }
    }
}