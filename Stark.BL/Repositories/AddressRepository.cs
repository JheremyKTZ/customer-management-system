using Stark.Common.Models;
using Stark.Generators.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Stark.BL.Repositories
{
    public class AddressRepository
    {
        private List<Address> _addresses = new List<Address>();
        private List<Customer> _customers = new List<Customer>();

        public AddressRepository()
        { }

        public AddressRepository(int quantity)
        {
            if (_addresses.Any())
            {
                return;
            }
            (_customers, _addresses) = PartialBuilder.CreateCustomers(quantity, quantity * 2);
        }

        public IList<Address> RetrieveAll()
        {
            return _addresses;
        }

        public Address Retrieve(int addressId)
        {
            return _addresses
                .FirstOrDefault(a => a.AddressId == addressId);
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
            _addresses.Add(address);
            return true;
        }

        public bool Delete(int addressId)
        {
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

            var foundedAddress = _addresses.FindIndex(a => a.AddressId == addressId);
            _addresses.RemoveAt(foundedAddress);

            return true;
        }
    }
}
