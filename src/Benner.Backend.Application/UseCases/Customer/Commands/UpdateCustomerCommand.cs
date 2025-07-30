using System;
using Benner.Backend.Domain.ValueObjects;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;

namespace Benner.Backend.Application.UseCases.Customer.Commands
{
    public class UpdateCustomerCommand : ICommand<Result<Domain.Entities.Customer>>
    {
        public UpdateCustomerCommand(Guid id, string name, string email, string phone, DateTime birthDate, Address address)
        {
            Id = id;
            Name = name;
            Email = email;
            Phone = phone;
            BirthDate = birthDate;
            Address = address;
        }

        public Guid Id { get; }
        public string Name { get; }
        public string Email { get; }
        public string Phone { get; }
        public DateTime BirthDate { get; }
        public Address Address { get; }
    }
}