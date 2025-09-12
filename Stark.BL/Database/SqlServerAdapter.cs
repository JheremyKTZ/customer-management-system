using Microsoft.EntityFrameworkCore;

namespace Stark.BL.Database
{
    public class SqlServerAdapter : IDatabaseAdapter
    {
        private readonly string _connectionString;
        public SqlServerAdapter(string connectionString)
        {
            _connectionString = connectionString;
        }

        public AppDbContext CreateContext(string datasetPathOrName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(_connectionString)
                .Options;
            return new AppDbContext(options);
        }
    }
}


