using System;
using Benner.Backend.Domain.ValueObjects;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;

namespace Benner.Backend.Application.UseCases.Customer.Commands
{
    public class CreateCustomerCommand : ICommand<Result<Domain.Entities.Customer>>
    {
        public CreateCustomerCommand(string name, string email, string phone, string document, DateTime birthDate, Address address)
        {
            Name = name;
            Email = email;
            Phone = phone;
            Document = document;
            BirthDate = birthDate;
            Address = address;
        }

        public string Name { get; }
        public string Email { get; }
        public string Phone { get; }
        public string Document { get; }
        public DateTime BirthDate { get; }
        public Address Address { get; }
    }
}