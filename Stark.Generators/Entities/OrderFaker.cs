using Bogus;
using Stark.Common;
using Stark.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stark.Generators.Entities
{
    internal class OrderFaker : Faker<Order>
    {
        public OrderFaker(IList<Customer> customers, IList<Product> products)
        {
            var customerData = customers
                .Select(c => new { c.CustomerId, Addresses = c.AddressList.Select(a => a.AddressId).ToList() })
                .ToDictionary(k => k.CustomerId, v => v.Addresses);

            RuleFor(o => o.OrderId, f => f.IndexGlobal + 1);
            RuleFor(o => o.OrderDate, f => f.Date.Between(DateTime.Now.AddYears(-1), DateTime.Now));
            RuleFor(o => o.EntityState, f => f.PickRandom(EntityStateOption.Active, EntityStateOption.Deleted));
            RuleFor(o => o.IsNew, f => f.Random.Bool());
            RuleFor(o => o.IsValid, f => true);
            RuleFor(o => o.CustomerId, f => f.Random.ListItem(
                customers.Select(c => c.CustomerId).ToList()));
            RuleFor(o => o.ShippingAddressId, (f, v) => f.Random.ListItem(customerData[v.CustomerId]));
            RuleFor(o => o.OrderItems, (f, o) =>
            {
                var product = f.Random.ListItem(products);
                return f.Make(
                    f.Random.Number(1, 5),
                    () => new OrderItem(f.IndexGlobal + 1, o.OrderId)
                    {
                        Quantity = f.Random.Int(1, 100),
                        ProductId = product.ProductId,
                        PurchasePrice = product.CurrentPrice * f.Random.Number(1, 2)
                    });
            });
        }
    }
}
