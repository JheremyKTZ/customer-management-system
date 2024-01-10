using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stark.BL.Repositories;

namespace Stark.BLTest
{
    [TestClass]
    public class CustomerRepositoryTest
    {
        [TestMethod]
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
