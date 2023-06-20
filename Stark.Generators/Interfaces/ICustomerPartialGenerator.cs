namespace Stark.Generators.Interfaces
{
    public interface ICustomerPartialGenerator : ICustomerGenerator
    {
        IBuildStubs GenerateOnlyCustomers(int quantity);
    }
}
