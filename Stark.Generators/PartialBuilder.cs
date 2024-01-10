using Stark.Common.Models;
using Stark.Generators.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Stark.Generators.Interfaces
{
    public class PartialBuilder
    {
        private class PartialDataGenerator :
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
            {
                _stubRecords.Customers = new CustomerFaker()
                    .Generate(quantity);
                _stubRecords.Addresses = _stubRecords.Customers
                    .SelectMany(c => c.AddressList)
                    .ToList();
            }

            private void ConfigureProductFaker(int quantity)
                => _stubRecords.Products = new ProductFaker()
                    .Generate(quantity);

            IBuildStubs ICustomerPartialGenerator.GenerateOnlyCustomers(int quantity)
            {
                ConfigureCustomerFaker(quantity);
                return this;
            }

            IProductGenerator ICustomerGenerator.GenerateCustomersAndAddresses(int quantity)
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
                _stubRecords.Orders = new OrderFaker(_stubRecords.Customers, _stubRecords.Products)
                    .Generate(quantity);

                return this;
            }

            StubRecords IBuildStubs.GenerateStubs()
            {
                return _stubRecords;
            }
        }

        public static (List<Customer>, List<Address>) CreateCustomers(
            int customerQuantity)
        {
            var stubs =
                ((ICustomerPartialGenerator)new PartialDataGenerator())
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
            int ordersQuantity)
        {
            var stubs =
                ((ICustomerGenerator)new PartialDataGenerator())
                .GenerateCustomersAndAddresses(customersQuantity)
                .GenerateProducts(productsQuantity)
                .GenerateOrders(ordersQuantity)
                .GenerateStubs();
            return (stubs.Orders.ToList(), stubs.Customers.ToList(), stubs.Products.ToList());
        }
    }
}