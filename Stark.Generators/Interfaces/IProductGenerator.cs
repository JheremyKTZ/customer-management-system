namespace Stark.Generators.Interfaces
{
    public interface IProductGenerator
    {
        IOrderGenerator GenerateProducts(int quantity);
    }
}
