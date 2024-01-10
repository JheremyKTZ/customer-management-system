namespace Stark.Generators.Interfaces
{
    public interface ICustomerGenerator
    {
        IProductGenerator GenerateCustomersAndAddresses(int quantity);
    }
}
