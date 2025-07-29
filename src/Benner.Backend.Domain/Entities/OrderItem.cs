using System.Xml.Serialization;
using Benner.Backend.Domain.Common;

namespace Benner.Backend.Domain.Entities;

[XmlRoot("OrderItem")]
public class OrderItem : BaseEntity
{
    protected OrderItem()
    {
    }

    public OrderItem(Guid orderId, Guid productId, string productName, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantity));

        if (unitPrice <= 0)
            throw new ArgumentException("Preço unitário deve ser maior que zero", nameof(unitPrice));

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Nome do produto é obrigatório", nameof(productName));

        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        CalculateTotalAmount();
    }

    [XmlElement("OrderId")]
    public Guid OrderId { get; private set; }

    [XmlElement("ProductId")]
    public Guid ProductId { get; private set; }

    [XmlElement("ProductName")]
    public string ProductName { get; private set; }

    [XmlElement("Quantity")]
    public int Quantity { get; private set; }

    [XmlElement("UnitPrice")]
    public decimal UnitPrice { get; private set; }

    [XmlElement("TotalAmount")]
    public decimal TotalAmount { get; private set; }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero", nameof(newQuantity));

        Quantity = newQuantity;
        CalculateTotalAmount();
        SetUpdatedAt();
    }

    public void UpdateUnitPrice(decimal newUnitPrice)
    {
        if (newUnitPrice <= 0)
            throw new ArgumentException("Preço unitário deve ser maior que zero", nameof(newUnitPrice));

        UnitPrice = newUnitPrice;
        CalculateTotalAmount();
        SetUpdatedAt();
    }

    private void CalculateTotalAmount()
    {
        TotalAmount = Quantity * UnitPrice;
    }
}