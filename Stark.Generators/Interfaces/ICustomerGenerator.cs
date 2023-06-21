namespace Stark.Generators.Interfaces
{
    public interface ICustomerGenerator
    {
        IProductGenerator GenerateCustomers(int quantity);
    }
}
