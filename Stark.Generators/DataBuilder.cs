using Stark.Generators.Entities;
using Stark.Generators.Interfaces;
using System.Linq;

namespace Stark.Generators
{
    public class DataBuilder
    {
        private class DataGenerator :
            IAddressGenerator, ICustomerGenerator, IProductGenerator, IOrderGenerator, IBuildStubs
        {
            private StubRecords _stubRecords;

            public DataGenerator()
            {
                _stubRecords = new StubRecords();
            }

            ICustomerGenerator IAddressGenerator.GenerateAddresses(int quantity)
            {
                _stubRecords.Addresses = new AddressFaker().Generate(quantity);
                return this;
            }

            IProductGenerator ICustomerGenerator.GenerateCustomers(int quantity)
            {
                _stubRecords.Customers = new CustomerFaker(_stubRecords.Addresses)
                    .Generate(quantity);

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
                var customerIds = _stubRecords.Customers
                    .Select(c => c.CustomerId)
                    .ToList();
                _stubRecords.Orders = new OrderFaker(customerIds, _stubRecords.Products)
                    .Generate(quantity);

                return this;
            }

            StubRecords IBuildStubs.GenerateStubs()
            {
                return _stubRecords;
            }
        }

        public static IAddressGenerator Create()
        {
            return new DataGenerator();
        }
    }
}
