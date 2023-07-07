using Bogus;
using Stark.Common;
using Stark.Common.Models;
using System.Collections.Generic;

namespace Stark.Generators.Entities
{
    internal class CustomerFaker : Faker<Customer>
    {
        public CustomerFaker(IList<Address> addresses)
        {
            RuleFor(c => c.CustomerId, f => f.IndexGlobal + 1);
            RuleFor(c => c.Email, f => f.Person.Email);
            RuleFor(c => c.FirstName, f => f.Person.FirstName);
            RuleFor(c => c.LastName, f => f.Person.LastName);
            RuleFor(c => c.EntityState, f => f.PickRandom(EntityStateOption.Active, EntityStateOption.Deleted));
            RuleFor(c => c.IsNew, f => f.Random.Bool());
            RuleFor(c => c.AddressList, f => 
            {
                var number = f.Random.Int(1, 3);
                return new List<Address>
                    (f.Random.ListItems(addresses, number));
            });
            RuleFor(c => c.CustomerType,
                f => f.PickRandom((int)CustomerType.Business, (int)CustomerType.Educator, (int)CustomerType.Residential, (int)CustomerType.Government));
            RuleFor(c => c.IsValid, f => true);
        }
    }
}