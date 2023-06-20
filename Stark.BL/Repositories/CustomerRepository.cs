using Stark.Common.Models;
using System;
using System.Linq;

namespace Stark.BL.Repositories
{
    public class CustomerRepository : IEquatable<CustomerRepository>
    {
        public CustomerRepository()
        {
            addressRepository = new AddressRepository();
        }

        private AddressRepository addressRepository { get; set; }

        public bool Equals(CustomerRepository other) => throw new NotImplementedException();

        public Customer Retrieve(int customerId)
        {
            Customer customer = new Customer(1);

            if (customerId == 1)
            {
                customer.Email = "pepperpotts@stark.com";
                customer.FirstName = "Pepper";
                customer.LastName = "Potts";
                customer.AddressList = addressRepository.RetrieveByCustomerId(customerId).ToList();
            }
            return customer;
        }

        public bool Save()
        {
            return true;
        }
    }
}
