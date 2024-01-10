using Stark.Common.Models;
using Stark.Generators.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Stark.BL.Repositories
{
    public class AddressRepository : IRepository<Address>
    {
        private List<Address> addresses = new List<Address>();
        private List<Customer> _customers = new List<Customer>();

        public AddressRepository(List<Address> addresses, List<Customer> customers)
        {
            this.addresses = addresses;
            _customers = customers;
        }

        public AddressRepository(int addressQuantity, int customersQuantity)
        {
            if (addresses.Any())
            {
                return;
            }

            (_customers, addresses) = PartialBuilder.CreateCustomers(customersQuantity);
        }

        public IList<Address> RetrieveAll()
        {
            return addresses;
        }

        public Address Retrieve(int addressId)
        {
            return addresses
                .FirstOrDefault(a => a.AddressId == addressId) ?? new Address();
        }

        public IEnumerable<Address> RetrieveByCustomerId(int customerId)
        {
            return _customers
                .Where(c => c.CustomerId == customerId)
                .SelectMany(c => c.AddressList)
                .ToList();
        }

        public bool Save(Address address)
        {
            if (address.Validate())
            {
                addresses.Add(address);
                return true;
            }
            
            return false;
        }

        public bool Delete(int addressId)
        {
            var foundedAddress = addresses.FindIndex(a => a.AddressId == addressId);
            if (foundedAddress == -1)
            {
                return false;
            }

            foreach (var customer in _customers)
            {
                foreach (var address in customer.AddressList)
                {
                    if (address.AddressId == addressId)
                    {
                        customer.AddressList.Remove(address);
                    }
                }
            }
            
            addresses.RemoveAt(foundedAddress);

            return true;
        }
    }
}
