namespace Stark.Generators.Interfaces
{
    public interface IAddressPartialGenerator : IAddressGenerator
    {
        IBuildStubs GenerateOnlyAddresses(int quantity);
    }
}
