using Bogus;
using Stark.Common;
using Stark.Common.Models;
using System;

namespace Stark.Generators.Entities
{
    internal class OrderFaker : Faker<Order>
    {
        public OrderFaker()
        {
            RuleFor(o => o.OrderId, f => f.IndexGlobal + 1);
            RuleFor(o => o.OrderDate, f => f.Date.Between(DateTime.Now.AddYears(-1), DateTime.Now));
            RuleFor(o => o.EntityState, f => f.PickRandom(EntityStateOption.Active, EntityStateOption.Deleted));
            RuleFor(o => o.IsNew, f => f.Random.Bool());
        }
    }
}
