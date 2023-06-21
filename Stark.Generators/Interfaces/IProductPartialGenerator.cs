namespace Stark.Generators.Interfaces
{
    public interface IProductPartialGenerator : IProductGenerator
    {
        IBuildStubs GenerateOnlyProducts(int quantity);
    }
}