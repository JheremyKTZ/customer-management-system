using Bogus;
using Models = Stark.Common.Models;

namespace Stark.Generators.Entities
{
    public class AddressFaker : Faker<Models.Address>
    {
        public AddressFaker(int customerId)
        {
            RuleFor(a => a.AddressId, f => f.IndexGlobal + 1);
            RuleFor(a => a.AddressType, f => f.PickRandom(new int[] { 1, 2, 3 }));
            RuleFor(a => a.City, f => f.Address.City());
            RuleFor(a => a.State, f => f.Address.State());
            RuleFor(a => a.Country, f => f.Address.Country());
            RuleFor(a => a.PostalCode, f => f.Address.ZipCode());
            RuleFor(a => a.AddressLine1, f => f.Address.StreetAddress());
            RuleFor(a => a.AddressLine2, f => f.Address.FullAddress());
            RuleFor(a => a.CustomerId, customerId);
        }
    }
}
