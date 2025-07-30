using System;
using System.Net.Mail;
using Benner.Backend.Domain.Common;
using Benner.Backend.Domain.Enumerators;
using Benner.Backend.Domain.ValueObjects;

namespace Benner.Backend.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public Customer()
        {
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
            Address = address;
            Status = CustomerStatus.Active;
        }

        private Customer(bool skipValidation)
        {
            if (!skipValidation)
                throw new InvalidOperationException("Use o construtor público ou CreateEmpty()");

            Name = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            Document = string.Empty;
            BirthDate = DateTime.Today;
            Address = Address.CreateEmpty();
            Status = CustomerStatus.Active;
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Document { get; set; }
        public DateTime BirthDate { get; set; }
        public Address Address { get; set; }
        public CustomerStatus Status { get; set; }

        public static Customer CreateEmpty()
        {
            return new Customer(true);
        }

        public void UpdateAddress(Address newAddress)
        {
            Address = newAddress;
        }

        public void UpdatePersonalInfo(string name, string email, string phone, DateTime birthDate)
        {
            ValidatePersonalInfo(name, email, phone, birthDate);

            Name = name;
            Email = email;
            Phone = phone;
            BirthDate = birthDate;
        }

        private void ValidatePersonalInfo(string name, string email, string phone, DateTime birthDate)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome é obrigatório", nameof(name));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email é obrigatório", nameof(email));

            if (!IsValidEmail(email))
                throw new ArgumentException("Email inválido", nameof(email));

            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Telefone é obrigatório", nameof(phone));

            if (birthDate >= DateTime.Today)
                throw new ArgumentException("Data de nascimento deve ser anterior à data atual", nameof(birthDate));

            if (birthDate < DateTime.Today.AddYears(-120))
                throw new ArgumentException("Data de nascimento inválida", nameof(birthDate));
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
}