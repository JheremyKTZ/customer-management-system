using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stark.Common.Models;

namespace Stark.CommonTest
{
    [TestClass]
    public class CustomerTests
    {
        [TestMethod]
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
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Customer_GivenEmptyFirstName_ReturnsOnlyLastNameInFullName()
        {
            // Arrange
            Customer customer = new Customer { LastName = "Stark" };
            string expected = "Stark";

            // Act
            string actual = customer.FullName;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Customer_GivenEmptyLastName_ReturnsOnlyFirstNameInFullName()
        {
            // Arrange
            Customer customer = new Customer { FirstName = "Tony" };
            string expected = "Tony";

            // Act
            string actual = customer.FullName;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
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
            Assert.AreEqual(3, Customer.InstanceCount);
        }

        [TestMethod]
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
            Assert.AreEqual(true, customer.Validate());

        }

        [TestMethod]
        public void CustomerValidate_GivenInvalidInformation_ShouldReturnFalse()
        {
            // Arrange
            Customer customer = new Customer
            {
                Email = "potts@stark.com"
            };

            // Act
            // Assert
            Assert.AreEqual(false, customer.Validate());
        }
    }
}
