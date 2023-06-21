using Stark.Common.Models;
using Stark.Generators;
using System.Collections.Generic;

namespace Stark.BL
{
    internal static class StubSeed
    {
        public static IList<Address> Addresses { get; private set; }
        public static IList<Customer> Customers { get; private set; }
        public static IList<Order> Orders { get; private set; }
        public static IList<Product> Products { get; private set; }

        public static void SeedUsingBuilder(int addressesQuantity, int customersQuantity, int productsQuantity, int ordersQuantity)
        {
            var stubs = DataBuilder
                .Create()
                .GenerateAddresses(addressesQuantity)
                .GenerateCustomers(customersQuantity)
                .GenerateProducts(productsQuantity)
                .GenerateOrders(ordersQuantity)
                .GenerateStubs();

            Addresses = stubs.Addresses;
            Customers = stubs.Customers;
            Orders = stubs.Orders;
            Products = stubs.Products;
        }
    }
}
