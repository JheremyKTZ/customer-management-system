using Stark.Generators.Entities;
using Stark.Generators.Interfaces;
using System.Linq;

namespace Stark.Generators
{
    public class DataBuilder
    {
        private class DataGenerator :
            ICustomerGenerator, IProductGenerator, IOrderGenerator, IBuildStubs
        {
            private StubRecords _stubRecords;

            public DataGenerator()
            {
                _stubRecords = new StubRecords();
            }

            IProductGenerator ICustomerGenerator.GenerateCustomersAndAddresses(int quantity)
            {
                _stubRecords.Customers = new CustomerFaker()
                    .Generate(quantity);
                _stubRecords.Addresses = _stubRecords.Customers
                    .SelectMany(c => c.AddressList)
                    .ToList();

                return this;
            }

            IOrderGenerator IProductGenerator.GenerateProducts(int quantity)
            {
                _stubRecords.Products = new ProductFaker()
                    .Generate(quantity);

                return this;
            }

            IBuildStubs IOrderGenerator.GenerateOrders(int quantity)
            {
                _stubRecords.Orders = new OrderFaker(_stubRecords.Customers, _stubRecords.Products)
                    .Generate(quantity);

                return this;
            }

            StubRecords IBuildStubs.GenerateStubs()
            {
                return _stubRecords;
            }
        }

        public static ICustomerGenerator Create()
        {
            return new DataGenerator();
        }
    }
}
