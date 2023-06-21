using Bogus;
using Stark.Common;
using Stark.Common.Models;

namespace Stark.Generators.Entities
{
    internal class ProductFaker : Faker<Product>
    {
        public ProductFaker()
        {
            RuleFor(p => p.ProductId, f => f.IndexGlobal + 1);
            RuleFor(p => p.ProductName, f => f.Commerce.Product());
            RuleFor(p => p.CurrentPrice, f => decimal.Parse(f.Commerce.Price()));
            RuleFor(p => p.Description, f => f.Commerce.ProductDescription());
            RuleFor(p => p.EntityState, f => f.PickRandom(EntityStateOption.Active, EntityStateOption.Deleted));
            RuleFor(p => p.IsNew, f => f.Random.Bool());
            RuleFor(p => p.IsValid, f => true);
        }
    }
}
