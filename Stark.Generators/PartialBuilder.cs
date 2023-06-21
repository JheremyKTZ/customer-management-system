using Stark.Common.Models;
using Stark.Generators.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Stark.Generators.Interfaces
{
    public class PartialBuilder
    {
        private class PartialDataGenerator :
            IAddressPartialGenerator,
            ICustomerPartialGenerator,
            IProductPartialGenerator,
            IOrderGenerator,
            IBuildStubs
        {
            private StubRecords _stubRecords;

            internal PartialDataGenerator()
            {
                _stubRecords = new StubRecords();
            }

            private void ConfigureCustomerFaker(int quantity)
                => _stubRecords.Customers = new CustomerFaker(_stubRecords.Addresses)
                    .Generate(quantity);

            private void ConfigureAddressFaker(int quantity)
                => _stubRecords.Addresses = new AddressFaker()
                    .Generate(quantity);

            private void ConfigureProductFaker(int quantity)
                => _stubRecords.Products = new ProductFaker()
                    .Generate(quantity);

            IBuildStubs IAddressPartialGenerator.GenerateOnlyAddresses(int quantity)
            {
                ConfigureAddressFaker(quantity);
                return this;
            }

            ICustomerGenerator IAddressGenerator.GenerateAddresses(int quantity)
            {
                ConfigureAddressFaker(quantity);
                return this;
            }

            IBuildStubs ICustomerPartialGenerator.GenerateOnlyCustomers(int quantity)
            {
                ConfigureCustomerFaker(quantity);
                return this;
            }

            IProductGenerator ICustomerGenerator.GenerateCustomers(int quantity)
            {
                ConfigureCustomerFaker(quantity);
                return this;
            }

            IOrderGenerator IProductGenerator.GenerateProducts(int quantity)
            {
                ConfigureProductFaker(quantity);
                return this;
            }

            IBuildStubs IProductPartialGenerator.GenerateOnlyProducts(int quantity)
            {
                ConfigureProductFaker(quantity);
                return this;
            }

            IBuildStubs IOrderGenerator.GenerateOrders(int quantity)
            {
                var customerIds = _stubRecords.Customers
                    .Select(c => c.CustomerId)
                    .ToList();
                _stubRecords.Orders = new OrderFaker(customerIds, _stubRecords.Products)
                    .Generate(quantity);

                return this;
            }

            StubRecords IBuildStubs.GenerateStubs()
            {
                return _stubRecords;
            }
        }

        public static IList<Address> CreateAddresses(int quantity)
        {
            var stubs =
                ((IAddressPartialGenerator)new PartialDataGenerator())
                    .GenerateOnlyAddresses(quantity)
                    .GenerateStubs();
            return stubs.Addresses;
        }

        public static (List<Customer>, List<Address>) CreateCustomers(
            int addressQuantity, int customerQuantity)
        {
            var stubs =
                ((ICustomerPartialGenerator)((IAddressPartialGenerator)new PartialDataGenerator())
                    .GenerateAddresses(addressQuantity))
                    .GenerateOnlyCustomers(customerQuantity)
                    .GenerateStubs();
            return (stubs.Customers.ToList(), stubs.Addresses.ToList());
        }

        public static List<Product> CreateProducts(int quantity)
        {
            var stubs =
                ((IProductPartialGenerator)new PartialDataGenerator())
                .GenerateOnlyProducts(quantity)
                .GenerateStubs();
            return stubs.Products.ToList();
        }

        public static (List<Order>, List<Customer>, List<Product>) CreateOrders(
            int customersQuantity,
            int productsQuantity,
            int ordersQuantity,
            int addressQuantity)
        {
            var stubs =
                ((IAddressGenerator)new PartialDataGenerator())
                .GenerateAddresses(addressQuantity)
                .GenerateCustomers(customersQuantity)
                .GenerateProducts(productsQuantity)
                .GenerateOrders(ordersQuantity)
                .GenerateStubs();
            return (stubs.Orders.ToList(), stubs.Customers.ToList(), stubs.Products.ToList());
        }
    }
}