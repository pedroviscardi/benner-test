using System.Xml.Serialization;
using Benner.Backend.Domain.Common;
using Benner.Backend.Domain.Enumerators;

namespace Benner.Backend.Domain.Entities;

[XmlRoot("Product")]
public class Product : BaseEntity
{
    public Product()
    {
    }

    public Product(string name,
        string description,
        string code,
        decimal price,
        int stockQuantity,
        int minimumStock,
        string category,
        string brand)
    {
        ValidateProductData(name, description, code, price, stockQuantity, minimumStock, category);

        Name = name;
        Description = description;
        Code = code;
        Price = price;
        StockQuantity = stockQuantity;
        MinimumStock = minimumStock;
        Category = category;
        Brand = brand;
        Status = ProductStatus.Active;
    }

    [XmlElement("Name")]
    public string Name { get; private set; }

    [XmlElement("Description")]
    public string Description { get; private set; }

    [XmlElement("Code")]
    public string Code { get; private set; }

    [XmlElement("Price")]
    public decimal Price { get; private set; }

    [XmlElement("StockQuantity")]
    public int StockQuantity { get; private set; }

    [XmlElement("MinimumStock")]
    public int MinimumStock { get; private set; }

    [XmlElement("Category")]
    public string Category { get; private set; }

    [XmlElement("Brand")]
    public string Brand { get; private set; }

    [XmlElement("Status")]
    public ProductStatus Status { get; private set; }

    public void UpdateInfo(string name, string description, decimal price, string category, string brand)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome é obrigatório", nameof(name));

        if (price <= 0)
            throw new ArgumentException("Preço deve ser maior que zero", nameof(price));

        Name = name;
        Description = description;
        Price = price;
        Category = category;
        Brand = brand;
        SetUpdatedAt();
    }

    public void UpdateStock(int newQuantity)
    {
        if (newQuantity < 0)
            throw new ArgumentException("Estoque não pode ser negativo", nameof(newQuantity));

        StockQuantity = newQuantity;
        SetUpdatedAt();
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantity));

        StockQuantity += quantity;
        SetUpdatedAt();
    }

    public void RemoveStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantity));

        if (StockQuantity < quantity)
            throw new InvalidOperationException("Estoque insuficiente");

        StockQuantity -= quantity;
        SetUpdatedAt();
    }

    public bool IsStockBelowMinimum()
    {
        return StockQuantity <= MinimumStock;
    }

    public void ChangeStatus(ProductStatus newStatus)
    {
        Status = newStatus;
        SetUpdatedAt();
    }

    private void ValidateProductData(string name,
        string description,
        string code,
        decimal price,
        int stockQuantity,
        int minimumStock,
        string category)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome é obrigatório", nameof(name));

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Código é obrigatório", nameof(code));

        if (price <= 0)
            throw new ArgumentException("Preço deve ser maior que zero", nameof(price));

        if (stockQuantity < 0)
            throw new ArgumentException("Estoque atual não pode ser negativo", nameof(stockQuantity));

        if (minimumStock < 0)
            throw new ArgumentException("Estoque mínimo não pode ser negativo", nameof(minimumStock));

        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Categoria é obrigatória", nameof(category));
    }
}