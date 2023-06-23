using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stark.Generators
{
    [TestClass]
    public class DataBuilderTests
    {
        [DataTestMethod]
        [DataRow(5, 3, 4, 6)]
        [DataRow(4, 5, 6, 8)]
        [DataRow(2, 3, 3, 4)]
        public void DataBuilderCreate_GivenEntitiesQuantity_ReturnsBuildedCollectionsOfEntities(
            int addressQuantity, int customersQuantity, int productsQuantity, int ordersQuantity)
        {
            var stubs = DataBuilder.Create()
                .GenerateAddresses(addressQuantity)
                .GenerateCustomers(customersQuantity)
                .GenerateProducts(productsQuantity)
                .GenerateOrders(ordersQuantity)
                .GenerateStubs();

            stubs.Addresses.Count.Should().Be(addressQuantity);
            stubs.Customers.Count.Should().Be(customersQuantity);
            stubs.Products.Count.Should().Be(productsQuantity);
            stubs.Orders.Count.Should().Be(ordersQuantity);
        }
    }
}
