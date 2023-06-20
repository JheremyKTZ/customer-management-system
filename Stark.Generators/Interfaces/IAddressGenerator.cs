namespace Stark.Generators.Interfaces
{
    public interface IAddressGenerator
    {
        ICustomerGenerator GenerateAddresses(int quantity);
    }
}
