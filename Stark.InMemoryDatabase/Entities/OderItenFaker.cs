using Bogus;
using Stark.Common.Models;

namespace Stark.Generators.Entities
{
    internal class OderItemFaker : Faker<OrderItem>
    {
        public OderItemFaker()
        {
            RuleFor(o => o.OrderItemId, f => f.IndexGlobal + 1);
            RuleFor(o => o.Quantity, f => f.Random.Int(1, 100));
        }
    }
}
