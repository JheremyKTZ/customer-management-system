namespace Stark.BL
{
    internal static class StubSeed
    {
        public static IList<Address> Addresses { get; private set; }
        public static IList<Customer> Customers { get; private set; }
        public static IList<Order> Orders { get; private set; }
        public static IList<Product> Products { get; private set; }

        public static void SeedUsingBuilder(int customersQuantity, int productsQuantity, int ordersQuantity)
        {
            var stubs = DataBuilder
                .Create()
                .GenerateCustomersAndAddresses(customersQuantity)
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
