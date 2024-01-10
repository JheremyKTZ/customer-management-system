using Bogus;
using Stark.Common.Models;
using System.Collections.Generic;
using System.Linq;

namespace Stark.Generators.Entities
{
    internal class OrderItemFaker : Faker<OrderItem>
    {
        public OrderItemFaker(int orderId, IList<Product> products)
        {
            RuleFor(o => o.OrderItemId, f => f.IndexGlobal + 1);
            RuleFor(o => o.OrderId, orderId);
            RuleFor(o => o.Quantity, f => f.Random.Int(1, 100));
            RuleFor(o => o.ProductId, f => f.Random.ListItem(products).ProductId);
            RuleFor(
                o => o.PurchasePrice,
                (f, o) => products
                    .FirstOrDefault(
                        p => o.ProductId == p.ProductId).CurrentPrice * f.Random.Decimal(1.1m, 1.5m));
        }
    }
}
