using Bogus;
using Stark.Common;
using Stark.Common.Models;
using System;
using System.Collections.Generic;

namespace Stark.Generators.Entities
{
    internal class OrderFaker : Faker<Order>
    {
        public OrderFaker(IList<int> customerIds, IList<Product> products)
        {
            RuleFor(o => o.OrderId, f => f.IndexGlobal + 1);
            RuleFor(o => o.OrderDate, f => f.Date.Between(DateTime.Now.AddYears(-1), DateTime.Now));
            RuleFor(o => o.EntityState, f => f.PickRandom(EntityStateOption.Active, EntityStateOption.Deleted));
            RuleFor(o => o.IsNew, f => f.Random.Bool());
            RuleFor(o => o.IsValid, f => true);
            RuleFor(o => o.CustomerId, f => f.Random.ListItem(customerIds));
            RuleFor(o => o.OrderItems, f =>
            {
                var product = f.Random.ListItem(products);
                return f.Make(
                    f.Random.Number(1, 5),
                    () => new OrderItem(f.IndexGlobal + 1)
                    {
                        Quantity = f.Random.Int(1, 100),
                        ProductId = product.ProductId,
                        PurchasePrice = product.CurrentPrice * f.Random.Number(1, 2)
                    });
            });
        }
    }
}
