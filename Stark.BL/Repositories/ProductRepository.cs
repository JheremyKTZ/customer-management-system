using Stark.Common.Models;
using Stark.Generators.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Stark.BL.Repositories
{
    public class ProductRepository : IRepository<Product>
    {
        private List<Product> _products = new List<Product>();

        public ProductRepository(int quantity)
        {
            if (_products.Any())
            {
                return;
            }

            (_products) = PartialBuilder
                .CreateProducts(quantity);
        }

        public ProductRepository(List<Product> products)
        {
            _products = products;
        }

        public Product Retrieve(int productId)
        {
            return _products
                .FirstOrDefault(a => a.ProductId == productId) ?? new Product();
        }

        public IList<Product> RetrieveAll() => _products;

        public bool Save(Product product)
        {
            if (!product.HasChanges)
            {
                return false;
            }

            if (!product.IsValid)
            {
                return false;
            }

            if (product.IsNew)
            {
                _products.Add(product);
                return true;
            }

            var productIndex = _products
                .FindIndex(a => a.ProductId == product.ProductId);
            if (productIndex == -1)
            {
                return false;
            }

            _products[productIndex] = product;
            return true;
        }

        public bool Delete(int productId)
        {
            var foundedProducts = _products.FindIndex(a => a.ProductId == productId);
            if (foundedProducts == -1)
            {
                return false;
            }

            _products.RemoveAt(foundedProducts);
            return true;
        }
    }
}
