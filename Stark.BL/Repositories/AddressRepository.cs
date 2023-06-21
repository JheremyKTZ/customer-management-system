using Stark.Common.Models;
using Stark.Generators.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Stark.BL.Repositories
{
    public class AddressRepository : IRepository<Address>
    {
        private List<Address> _addresses = new List<Address>();
        private List<Customer> _customers = new List<Customer>();

        public AddressRepository(List<Address> addresses, List<Customer> customers)
        {
            _addresses = addresses;
            _customers = customers;
        }

        public AddressRepository(int addressQuantity, int customersQuantity)
        {
            if (_addresses.Any())
            {
                return;
            }

            (_customers, _addresses) = PartialBuilder.CreateCustomers(addressQuantity, customersQuantity);
        }

        public IList<Address> RetrieveAll()
        {
            return _addresses;
        }

        public Address Retrieve(int addressId)
        {
            return _addresses
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
                _addresses.Add(address);
                return true;
            }
            
            return false;
        }

        public bool Delete(int addressId)
        {
            var foundedAddress = _addresses.FindIndex(a => a.AddressId == addressId);
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
            
            _addresses.RemoveAt(foundedAddress);

            return true;
        }
    }
}
