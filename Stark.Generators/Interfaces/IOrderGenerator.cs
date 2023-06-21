namespace Stark.Generators.Interfaces
{
    public interface IOrderGenerator
    {
        IBuildStubs GenerateOrders(int quantity);
    }
}
