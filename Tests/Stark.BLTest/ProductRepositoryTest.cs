using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stark.BL.Repositories;
using Stark.Common.Models;
using System.Linq;

namespace Stark.BLTest
{
    [TestClass]
    public class ProductRepositoryTest
    {
        [TestMethod]
        public void RetrieveAll_WhenRepositoryHasInformation_ReturnsCountForAllProducts()
        {
            // Arrange
            var expectedProducts = 2;
            ProductRepository productRepository = new ProductRepository(expectedProducts);

            // Act
            var products = productRepository.RetrieveAll();
            
            // Assert
            products.Count.Should().Be(expectedProducts);
        }

        [TestMethod()]
        public void Save_GivenExistingProduct_UpdatesProductReturnsTrue()
        {
            // Arrange
            var expectedProducts = 2;
            var productRepository = new ProductRepository(expectedProducts);

            var productToUpdate = productRepository.RetrieveAll().First();

            var updatedProduct = new Product(productToUpdate.ProductId)
            {
                CurrentPrice = 18M,
                ProductName = "Bat mobile",
                Description = "Best vehicle in the world.",
                HasChanges = true,
            };

            // Act
            var savedSucceded = productRepository.Save(updatedProduct);

            // Assert
            savedSucceded.Should().BeTrue();
        }
        
        [TestMethod()]
        public void Save_GivenNonExistingProduct_UpdatesProductReturnsFalse()
        {
            // Arrange
            var expectedProducts = 2;
            var productRepository = new ProductRepository(expectedProducts);
            var updatedProduct = new Product(4)
            {
                CurrentPrice = null,
                Description = "Assorted size set of 4 bright yellow mini sunflowers.",
                ProductName = "Sunflowers",
                HasChanges = true,
            };

            // Act
            var savedSucceded = productRepository.Save(updatedProduct);

            // Assert
            savedSucceded.Should().BeFalse();
        }
    }
}
