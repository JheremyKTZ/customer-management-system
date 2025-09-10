using Xunit;
using Stark.Common.Models;

namespace Stark.CommonTest
{
    public class CustomerTests
    {
        [Fact]
        public void Customer_GivenValidFullName_ReturnsExpectedNameFormat()
        {
            // Arrange
            Customer customer = new Customer
            {
                FirstName = "Tony",
                LastName = "Stark"
            };
            string expected = "Stark, Tony";

            // Act
            string actual = customer.FullName;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Customer_GivenEmptyFirstName_ReturnsOnlyLastNameInFullName()
        {
            // Arrange
            Customer customer = new Customer { LastName = "Stark" };
            string expected = "Stark";

            // Act
            string actual = customer.FullName;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Customer_GivenEmptyLastName_ReturnsOnlyFirstNameInFullName()
        {
            // Arrange
            Customer customer = new Customer { FirstName = "Tony" };
            string expected = "Tony";

            // Act
            string actual = customer.FullName;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CustomerInstanceCount_GivenMultipleCustomers_ShouldReturnTheCount()
        {
            // Arrange
            Customer.InstanceCount = 0;
            Customer ob1 = new Customer();
            Customer.InstanceCount++;
            Customer ob2 = new Customer();
            Customer.InstanceCount++;
            Customer ob3 = new Customer();
            Customer.InstanceCount++;

            // Act
            // Assert
            Assert.Equal(3, Customer.InstanceCount);
        }

        [Fact]
        public void CustomerValidate_GivenValidInformation_ShouldReturnTrue()
        {
            // Arrange
            Customer customer = new Customer
            {
                LastName = "Potts",
                Email = "potts@stark.com"
            };

            // Act
            // Assert
            Assert.True(customer.Validate());
        }

        [Fact]
        public void CustomerValidate_GivenInvalidInformation_ShouldReturnFalse()
        {
            // Arrange
            Customer customer = new Customer
            {
                Email = "potts@stark.com"
            };

            // Act
            // Assert
            Assert.False(customer.Validate());
        }
    }
}
