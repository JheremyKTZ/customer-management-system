using Stark.Common.Models;
using System.Collections.Generic;

namespace Stark.Generators
{
    public class StubRecords
    {
        public IList<Customer> Customers { get; internal set; }
        public IList<Order> Orders { get; internal set; }
        public IList<Address> Addresses { get; internal set; }
        public IList<Product> Products { get; internal set; }
    }
}
