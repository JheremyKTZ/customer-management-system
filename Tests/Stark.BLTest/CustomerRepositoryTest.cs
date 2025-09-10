using FluentAssertions;
using Xunit;
using Stark.BL.Repositories;

namespace Stark.BLTest
{
    public class CustomerRepositoryTest
    {
        [Fact]
        public void RetrievesAll_WhenCustomersExist_ReturnsExpectedCountOfCustomers()
        {
            // Arrange
            var expectedCustomers = 2;
            CustomerRepository customerRepository = new CustomerRepository(expectedCustomers);

            // Act
            var customers = customerRepository.RetrieveAll();

            // Assert
            customers.Count.Should().Be(expectedCustomers);
        }
    }
}
