using Microsoft.EntityFrameworkCore;

namespace Stark.BL.Database
{
    public class SqliteAdapter : IDatabaseAdapter
    {
        private readonly string _defaultPath;
        public SqliteAdapter(string defaultPath)
        {
            _defaultPath = defaultPath;
        }

        public AppDbContext CreateContext(string datasetPathOrName)
        {
            var path = _defaultPath; // always use the configured single DB file
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite($"Data Source={path}")
                .Options;
            return new AppDbContext(options);
        }
    }
}


