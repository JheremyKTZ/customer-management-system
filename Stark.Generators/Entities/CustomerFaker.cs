using Bogus;
using Stark.Common;
using Stark.Common.Models;

namespace Stark.Generators.Entities
{
    internal class CustomerFaker : Faker<Customer>
    {
        public CustomerFaker()
        {
            RuleFor(c => c.CustomerId, f => f.IndexGlobal + 1);
            RuleFor(c => c.Email, f => f.Person.Email);
            RuleFor(c => c.FirstName, f => f.Person.FirstName);
            RuleFor(c => c.LastName, f => f.Person.LastName);
            RuleFor(c => c.EntityState, f => f.PickRandom(EntityStateOption.Active, EntityStateOption.Deleted));
            RuleFor(c => c.IsNew, f => f.Random.Bool());
            RuleFor(c => c.CustomerType,
                f => f.PickRandom((int)CustomerType.Business, (int)CustomerType.Educator, (int)CustomerType.Residential, (int)CustomerType.Government));
            RuleFor(c => c.IsValid, f => true);
            RuleFor(c => c.AddressList, (f, c) =>
            {
                var address = new AddressFaker(c.CustomerId);
                return address.GenerateBetween(1, 3);
            });
        }
    }
}