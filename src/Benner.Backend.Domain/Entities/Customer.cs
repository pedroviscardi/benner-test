using System.Net.Mail;
using System.Xml.Serialization;
using Benner.Backend.Domain.Common;
using Benner.Backend.Domain.Enumerators;
using Benner.Backend.Domain.ValueObjects;

namespace Benner.Backend.Domain.Entities;

[XmlRoot("Customer")]
public class Customer : BaseEntity
{
    public Customer()
    {
        Orders = [];
    }

    public Customer(string name,
        string email,
        string phone,
        string document,
        DateTime birthDate,
        Address address)
    {
        ValidateCustomerData(name, email, phone, document, birthDate);

        Name = name;
        Email = email;
        Phone = phone;
        Document = document;
        BirthDate = birthDate;
        Address = address ?? throw new ArgumentNullException(nameof(address), "Endereço é obrigatório");
        Status = CustomerStatus.Active;
        Orders = [];
    }

    [XmlElement("Name")]
    public string Name { get; private set; }

    [XmlElement("Email")]
    public string Email { get; private set; }

    [XmlElement("Phone")]
    public string Phone { get; private set; }

    [XmlElement("Document")]
    public string Document { get; private set; }

    [XmlElement("BirthDate")]
    public DateTime BirthDate { get; private set; }

    [XmlElement("Status")]
    public CustomerStatus Status { get; private set; }

    [XmlElement("Address")]
    public Address Address { get; private set; }

    [XmlArray("Orders")]
    [XmlArrayItem("Order")]
    public List<Order> Orders { get; private set; }

    public void UpdatePersonalInfo(string name, string email, string phone, DateTime birthDate)
    {
        ValidateCustomerData(name, email, phone, Document, birthDate);

        Name = name;
        Email = email;
        Phone = phone;
        BirthDate = birthDate;
        SetUpdatedAt();
    }

    public void UpdateAddress(Address newAddress)
    {
        Address = newAddress ?? throw new ArgumentNullException(nameof(newAddress), "Endereço é obrigatório");
        SetUpdatedAt();
    }

    public void ChangeStatus(CustomerStatus newStatus)
    {
        Status = newStatus;
        SetUpdatedAt();
    }

    public void AddOrder(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order), "Pedido não pode ser nulo");

        Orders.Add(order);
        SetUpdatedAt();
    }

    public void RemoveOrder(Guid orderId)
    {
        var order = Orders.Find(o => o.Id == orderId);
        if (order != null)
        {
            Orders.Remove(order);
            SetUpdatedAt();
        }
    }

    private void ValidateCustomerData(string name, string email, string phone, string document, DateTime birthDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome é obrigatório", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email é obrigatório", nameof(email));

        if (!IsValidEmail(email))
            throw new ArgumentException("Email inválido", nameof(email));

        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Telefone é obrigatório", nameof(phone));

        if (string.IsNullOrWhiteSpace(document))
            throw new ArgumentException("Documento é obrigatório", nameof(document));

        if (birthDate >= DateTime.Today)
            throw new ArgumentException("Data de nascimento deve ser anterior à data atual", nameof(birthDate));

        if (birthDate < DateTime.Today.AddYears(-120))
            throw new ArgumentException("Data de nascimento inválida", nameof(birthDate));
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}