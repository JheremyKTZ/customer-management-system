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
            RuleFor(c => c.AddressList, f => new List<Address>
                (f.Random.ListItems(addresses)));
            RuleFor(c => c.IsValid, f => true);
        }
    }
}