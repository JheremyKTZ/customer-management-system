using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stark.BL.Repositories;

namespace Stark.BLTest
{
    [TestClass]
    public class OrderRepositoryTest
    {
        [TestMethod]
        public void RetrieveValid()
        {
            // Arrange
            var ordersExpected = 2;
            OrderRepository orderRepository = new OrderRepository(ordersExpected);

            // Act
            var orders = orderRepository.RetrieveAll();
            
            // Assert
            orders.Count.Should().Be(ordersExpected);
        }
    }
}
