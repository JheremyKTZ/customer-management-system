﻿namespace Stark.Common.Models
{
    public class Address
    {
        public Address()
        {
        }

        public Address(int addressId)
        {
            AddressId = addressId;
        }

        public int AddressId { get; private set; }
        public int CustomerId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public int AddressType { get; set; }

        public bool Validate()
        {
            bool IsValid = PostalCode == null ? false : true;
            return IsValid;
        }

        // Navegación a Customer
        public Customer Customer { get; set; }
    }
}
