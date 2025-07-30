using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Benner.Backend.Domain.Common;

namespace Benner.Backend.Domain.ValueObjects
{
    public class Address : ValueObject
    {
        private Address()
        {
        }

        public Address(string street,
            string number,
            string neighborhood,
            string city,
            string state,
            string postalCode,
            string country = "Brasil",
            string complement = null)
        {
            ValidateAddressData(street, number, neighborhood, city, state, postalCode, country);

            Street = street;
            Number = number;
            Complement = complement;
            Neighborhood = neighborhood;
            City = city;
            State = state;
            PostalCode = CleanPostalCode(postalCode);
            Country = country;
        }

        private Address(bool skipValidation)
        {
            if (!skipValidation)
                throw new InvalidOperationException("Use o construtor público ou CreateEmpty()");

            Street = string.Empty;
            Number = string.Empty;
            Neighborhood = string.Empty;
            City = string.Empty;
            State = string.Empty;
            PostalCode = string.Empty;
            Country = string.Empty;
            Complement = string.Empty;
        }


        [XmlElement("Street")]
        public string Street { get; set; }

        [XmlElement("Number")]
        public string Number { get; set; }

        [XmlElement("Complement")]
        public string Complement { get; set; }

        [XmlElement("Neighborhood")]
        public string Neighborhood { get; set; }

        [XmlElement("City")]
        public string City { get; set; }

        [XmlElement("State")]
        public string State { get; set; }

        [XmlElement("PostalCode")]
        public string PostalCode { get; set; }

        [XmlElement("Country")]
        public string Country { get; set; }

        public string FullAddress
        {
            get
            {
                var address = $"{Street}, {Number}";
                if (!string.IsNullOrWhiteSpace(Complement))
                    address += $", {Complement}";
                address += $" - {Neighborhood}, {City}/{State} - CEP: {FormattedPostalCode} - {Country}";
                return address;
            }
        }

        public string FormattedPostalCode =>
            PostalCode.Length == 8 ? $"{PostalCode.Substring(0, 5)}-{PostalCode.Substring(5, 3)}" : PostalCode;

        public static Address CreateEmpty()
        {
            return new Address(true);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Street;
            yield return Number;
            yield return Complement ?? string.Empty;
            yield return Neighborhood;
            yield return City;
            yield return State;
            yield return PostalCode;
            yield return Country;
        }

        private void ValidateAddressData(string street,
            string number,
            string neighborhood,
            string city,
            string state,
            string postalCode,
            string country)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new ArgumentException("Logradouro é obrigatório", nameof(street));

            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Número é obrigatório", nameof(number));

            if (string.IsNullOrWhiteSpace(neighborhood))
                throw new ArgumentException("Bairro é obrigatório", nameof(neighborhood));

            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("Cidade é obrigatória", nameof(city));

            if (string.IsNullOrWhiteSpace(state))
                throw new ArgumentException("Estado é obrigatório", nameof(state));

            if (string.IsNullOrWhiteSpace(postalCode))
                throw new ArgumentException("CEP é obrigatório", nameof(postalCode));

            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentException("País é obrigatório", nameof(country));

            var cleanPostalCode = CleanPostalCode(postalCode);
            if (cleanPostalCode.Length != 8)
                throw new ArgumentException("CEP deve ter 8 dígitos", nameof(postalCode));
        }

        private string CleanPostalCode(string postalCode)
        {
            return postalCode.Replace("-", "").Replace(".", "").Trim();
        }

        public override string ToString()
        {
            return FullAddress;
        }
    }
}