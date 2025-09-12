using Stark.BL;

namespace Stark.BL.Database
{
    public interface IDatabaseAdapter
    {
        AppDbContext CreateContext(string datasetPathOrName);
    }
}


