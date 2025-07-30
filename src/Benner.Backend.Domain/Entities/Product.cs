using System;
using Benner.Backend.Domain.Common;

namespace Benner.Backend.Domain.Entities
{
    public class Product : BaseEntity
    {
        public Product()
        {
        }

        public Product(string name, string description, string code, decimal price, int stockQuantity, int minimumStock, string category, string brand)
        {
            ValidateProductData(name, description, code, price, stockQuantity, minimumStock, category, brand);

            Name = name;
            Description = description;
            Code = code;
            Price = price;
            StockQuantity = stockQuantity;
            MinimumStock = minimumStock;
            Category = category;
            Brand = brand;
        }

        private Product(bool skipValidation)
        {
            if (!skipValidation)
                throw new InvalidOperationException("Use o construtor público ou CreateEmpty()");

            Name = string.Empty;
            Description = string.Empty;
            Code = string.Empty;
            Price = 0;
            StockQuantity = 0;
            MinimumStock = 0;
            Category = string.Empty;
            Brand = string.Empty;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int MinimumStock { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }

        public static Product CreateEmpty()
        {
            return new Product(true);
        }

        public void UpdateInfo(string name, string description, decimal price, string category, string brand)
        {
            Name = name;
            Description = description;
            Price = price;
            Category = category;
            Brand = brand;
        }

        public void UpdateStock(int newQuantity)
        {
            StockQuantity = newQuantity;
        }

        public bool IsStockBelowMinimum()
        {
            return StockQuantity < MinimumStock;
        }

        private void ValidateProductData(string name, string description, string code, decimal price, int stockQuantity, int minimumStock, string category, string brand)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome é obrigatório", nameof(name));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Descrição é obrigatória", nameof(description));

            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Código é obrigatório", nameof(code));

            if (price < 0)
                throw new ArgumentException("Preço deve ser maior ou igual a zero", nameof(price));

            if (stockQuantity < 0)
                throw new ArgumentException("Quantidade em estoque deve ser maior ou igual a zero", nameof(stockQuantity));

            if (minimumStock < 0)
                throw new ArgumentException("Estoque mínimo deve ser maior ou igual a zero", nameof(minimumStock));

            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Categoria é obrigatória", nameof(category));

            if (string.IsNullOrWhiteSpace(brand))
                throw new ArgumentException("Marca é obrigatória", nameof(brand));
        }
    }
}