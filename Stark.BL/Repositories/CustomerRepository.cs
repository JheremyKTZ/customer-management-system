using Stark.Common.Models;
using Stark.Generators.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Stark.BL.Repositories
{
    public class CustomerRepository : IRepository<Customer>
    {
        private List<Customer> _customers = new List<Customer>();

        public CustomerRepository(List<Customer> customers)
        {
            _customers = customers;
        }

        public CustomerRepository(int customersQuantity, int addressesQuantity)
        {
            if (_customers.Any())
            {
                return;
            }

            (_customers, _) = PartialBuilder
                .CreateCustomers(addressesQuantity, customersQuantity);
        }

        public IList<Customer> RetrieveAll() => _customers;

        public Customer Retrieve(int customerId)
        {
            return _customers
                .FirstOrDefault(a => a.CustomerId == customerId) ?? new Customer();
        }

        public bool Delete(int customerId)
        {
            var foundedCustomer = _customers.FindIndex(a => a.CustomerId == customerId);
            if (foundedCustomer == -1)
            {
                return false;
            }

            _customers.RemoveAt(foundedCustomer);
            return true;
        }

        public bool Save(Customer newCustomer)
        {
            if (!newCustomer.HasChanges)
            {
                return false;
            }

            if (!newCustomer.IsValid)
            {
                return false;
            }

            if (newCustomer.IsNew)
            {
                _customers.Add(newCustomer);
                return true;
            }

            var customerIndex = _customers
                .FindIndex(a => a.CustomerId == newCustomer.CustomerId);
            if (customerIndex == -1)
            {
                return false;
            }

            _customers[customerIndex] = newCustomer;
            return true;
        }
    }
}
