using Stark.Common.Models;

namespace Stark.Generators
{
    public class StubRecords
    {
        public IList<Customer>? Customers { get; set; }
        public IList<Order>? Orders { get; set; }
        public IList<Address>? Addresses { get; set; }
        public IList<Product>? Products { get; set; }
    }
}
