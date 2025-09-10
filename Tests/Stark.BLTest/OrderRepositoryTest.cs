using FluentAssertions;
using Xunit;
using Stark.BL.Repositories;

namespace Stark.BLTest
{
    public class OrderRepositoryTest
    {
        [Fact]
        public void RetrieveAll_WhenOrderExist_ReturnsExpectedCountOfOrders()
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
