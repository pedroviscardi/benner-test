using System.Xml.Serialization;
using Benner.Backend.Domain.Common;

namespace Benner.Backend.Domain.Entities;

[XmlRoot("Order")]
public class Order : BaseEntity
{
    public Order()
    {
        Items = [];
    }

    public Order(Guid customerId, string notes = null)
    {
        CustomerId = customerId;
        OrderNumber = GenerateOrderNumber();
        OrderDate = DateTime.UtcNow;
        Status = OrderStatus.Draft;
        TotalAmount = 0;
        DiscountAmount = 0;
        FinalAmount = 0;
        Notes = notes;
        Items = [];
    }

    [XmlElement("OrderNumber")]
    public string OrderNumber { get; private set; }

    [XmlElement("CustomerId")]
    public Guid CustomerId { get; private set; }

    [XmlElement("OrderDate")]
    public DateTime OrderDate { get; private set; }

    [XmlElement("Status")]
    public OrderStatus Status { get; private set; }

    [XmlElement("TotalAmount")]
    public decimal TotalAmount { get; private set; }

    [XmlElement("DiscountAmount")]
    public decimal DiscountAmount { get; private set; }

    [XmlElement("FinalAmount")]
    public decimal FinalAmount { get; private set; }

    [XmlElement("Notes")]
    public string Notes { get; private set; }

    [XmlArray("Items")]
    [XmlArrayItem("OrderItem")]
    public List<OrderItem> Items { get; private set; }

    public void AddItem(Product product, int quantity, decimal unitPrice)
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Não é possível adicionar itens a um pedido que não está em rascunho");

        if (product == null)
            throw new ArgumentNullException(nameof(product), "Produto não pode ser nulo");

        if (quantity <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantity));

        if (unitPrice <= 0)
            throw new ArgumentException("Preço unitário deve ser maior que zero", nameof(unitPrice));

        var existingItem = Items.FirstOrDefault(i => i.ProductId == product.Id);

        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            var newItem = new OrderItem(Id, product.Id, product.Name, quantity, unitPrice);
            Items.Add(newItem);
        }

        RecalculateAmounts();
    }

    public void RemoveItem(Guid productId)
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Não é possível remover itens de um pedido que não está em rascunho");

        var item = Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            Items.Remove(item);
            RecalculateAmounts();
        }
    }

    public void ApplyDiscount(decimal discountAmount)
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Não é possível aplicar desconto a um pedido que não está em rascunho");

        if (discountAmount < 0)
            throw new ArgumentException("Valor do desconto não pode ser negativo", nameof(discountAmount));

        if (discountAmount > TotalAmount)
            throw new ArgumentException("Valor do desconto não pode ser maior que o valor total", nameof(discountAmount));

        DiscountAmount = discountAmount;
        RecalculateAmounts();
    }

    public void ConfirmOrder()
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Apenas pedidos em rascunho podem ser confirmados");

        if (!Items.Any())
            throw new InvalidOperationException("Não é possível confirmar um pedido sem itens");

        Status = OrderStatus.Confirmed;
        SetUpdatedAt();
    }

    public void CancelOrder(string reason)
    {
        if (Status == OrderStatus.Delivered)
            throw new InvalidOperationException("Não é possível cancelar um pedido já entregue");

        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Pedido já está cancelado");

        Status = OrderStatus.Cancelled;
        Notes = $"{Notes}\nCancelado: {reason}";
        SetUpdatedAt();
    }

    public void MarkAsDelivered()
    {
        if (Status != OrderStatus.InProgress)
            throw new InvalidOperationException("Apenas pedidos em andamento podem ser marcados como entregues");

        Status = OrderStatus.Delivered;
        SetUpdatedAt();
    }

    private void RecalculateAmounts()
    {
        TotalAmount = Items.Sum(i => i.TotalAmount);
        FinalAmount = TotalAmount - DiscountAmount;
        SetUpdatedAt();
    }

    private string GenerateOrderNumber()
    {
        return $"ORD{DateTime.UtcNow:yyyyMMddHHmmss}";
    }
}

public enum OrderStatus
{
    Draft = 1,
    Confirmed = 2,
    InProgress = 3,
    Delivered = 4,
    Cancelled = 5
}