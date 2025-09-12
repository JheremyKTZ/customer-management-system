using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Stark.BL;
using Stark.BL.Database;
using Stark.Common.Models;
using Stark.Generators;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Data.SqlClient;

namespace LINQ.Playground
{
    class Program
    {
        private static string? _currentDataset = null;
        private static List<DatasetInfo> _datasets = new List<DatasetInfo>();
        private static readonly string ProjectDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../"));
        private static string GetDataFilePath(string name) => Path.Combine(ProjectDir, name);

        static void Main(string[] args)
        {
            Console.WriteLine("=== STARK CUSTOMER MANAGEMENT SYSTEM ===");
            Console.WriteLine("LINQ demonstration system with Entity Framework Core");
            Console.WriteLine();

            LoadConfiguration();
            LoadDatasets();
            ShowMainMenu();
        }
        private static IConfigurationRoot? _configuration;

        // Adapter selector
        private static IDatabaseAdapter GetAdapter()
        {
            var provider = GetCurrentProvider();
            if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                var cs = _configuration?["ConnectionStrings:SqlServer"] ?? "Server=localhost\\SQLEXPRESS;Database=StarkDb;Trusted_Connection=True;TrustServerCertificate=True";
                return new SqlServerAdapter(cs);
            }

            // default to SQLite
            var configuredPath = _configuration?["ConnectionStrings:SqlitePath"] ?? "stark_database.db";
            // If path is relative, place it under project directory (LINQ.Playground)
            var sqlitePath = Path.IsPathRooted(configuredPath) ? configuredPath : GetDataFilePath(configuredPath);
            return new SqliteAdapter(sqlitePath);
        }

        private static string GetCurrentProvider()
        {
            return (_configuration?["Database:Provider"] ?? "Sqlite").Trim();
        }

        private static void LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(ProjectDir)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            _configuration = builder.Build();
        }

        private static void ShowMainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== MAIN MENU ===");
                Console.WriteLine($"Current provider: {GetCurrentProvider()}");
                Console.WriteLine($"Current dataset: {(_currentDataset ?? "None")}");
                Console.WriteLine();
                Console.WriteLine("1. Manage data");
                Console.WriteLine("2. Execute exercises");
                Console.WriteLine("3. Exit");
                Console.WriteLine();
                Console.Write("Select an option: ");

                var choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        ShowDataManagementMenu();
                        break;
                    case "2":
                        ShowExercisesMenu();
                        break;
                    case "3":
                        Console.WriteLine("See you later!");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Press Enter to continue...");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private static void ShowDataManagementMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== DATA MANAGEMENT ===");
                Console.WriteLine();
                Console.WriteLine("1. Generate data");
                Console.WriteLine("2. Load data from current provider");
                Console.WriteLine("3. Delete old datasets");
                Console.WriteLine("4. Return to main menu");
                Console.WriteLine();
                Console.Write("Select an option: ");

                var choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        GenerateData();
                        break;
                    case "2":
                        LoadDataFromCurrentProvider();
                        break;
                    case "3":
                        DeleteOldDatasets();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Press Enter to continue...");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private static void GenerateData()
        {
            Console.WriteLine("=== GENERATE DATA ===");
            Console.WriteLine();
            Console.WriteLine("Select the amount of data to generate:");
            Console.WriteLine("1. 0 data (structure only)");
            Console.WriteLine("2. 50 data");
            Console.WriteLine("3. 100 data");
            Console.WriteLine("4. 150 data");
            Console.WriteLine("5. 200 data");
            Console.WriteLine();
            Console.Write("Select an option: ");

            var choice = Console.ReadLine();
            int dataCount = choice switch
            {
                "1" => 0,
                "2" => 50,
                "3" => 100,
                "4" => 150,
                "5" => 200,
                _ => 100
            };

            Console.WriteLine();
            Console.Write("Enter the name for the dataset (e.g: Dataset A): ");
            var datasetName = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(datasetName))
            {
                datasetName = $"Dataset_{DateTime.Now:yyyyMMdd_HHmmss}";
            }

            try
            {
                Console.WriteLine();
                Console.WriteLine("Generating data...");
                
                var stubRecords = Playground.GenerateDataWithFallback(dataCount, dataCount, dataCount);
                
                Console.WriteLine($"Data generated:");
                Console.WriteLine($" - Customers: {stubRecords.Customers?.Count ?? 0}");
                Console.WriteLine($" - Products: {stubRecords.Products?.Count ?? 0}");
                Console.WriteLine($" - Addresses: {stubRecords.Addresses?.Count ?? 0}");
                Console.WriteLine($" - Orders: {stubRecords.Orders?.Count ?? 0}");
                Console.WriteLine();

                var provider = GetCurrentProvider();
                var totalRecords = (stubRecords.Customers?.Count ?? 0) +
                                   (stubRecords.Products?.Count ?? 0) +
                                   (stubRecords.Addresses?.Count ?? 0) +
                                   (stubRecords.Orders?.Count ?? 0);

                // Create dataset info for the single database per provider
                var datasetInfo = new DatasetInfo
                {
                    Name = datasetName,
                    Provider = provider,
                    CreatedDate = DateTime.Now,
                    RecordCount = totalRecords,
                    IsGenerated = true
                };

                // Persist metadata tagged with current provider
                _datasets.Add(datasetInfo);
                SaveDatasetsInfo();

                try
                {
                    SaveToDatabase(stubRecords, "", provider);
                }
                catch (Exception ex)
                {
                    // Rollback metadata if persisting data failed
                    _datasets.RemoveAll(d => d.Name == datasetName && (d.Provider ?? "").Equals(provider, StringComparison.OrdinalIgnoreCase));
                    SaveDatasetsInfo();
                    var inner = GetInnermostExceptionMessage(ex);
                    throw new Exception($"Error saving dataset to storage: {ex.Message}. Inner: {inner}");
                }

                // Set the new dataset as current
                _currentDataset = datasetName;
                SaveCurrentDataset();

                var location = provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase)
                    ? "SQL Server database 'StarkDb'"
                    : "SQLite file 'stark_database.db'";
                Console.WriteLine($"Dataset '{datasetName}' saved successfully to {location}");
                Console.WriteLine($"Total records: {totalRecords}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating data: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        private static void LoadDataFromCurrentProvider()
        {
            Console.WriteLine("=== LOAD DATA FROM CURRENT PROVIDER ===");
            Console.WriteLine();
            
            var provider = GetCurrentProvider();
            Console.WriteLine($"Current provider: {provider}");
            Console.WriteLine();
            
            var providerDatasets = _datasets
                .Where(d => string.Equals((d.Provider ?? "Sqlite").Trim(), provider, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (providerDatasets.Count == 0)
            {
                Console.WriteLine($"No datasets available for {provider} provider.");
                Console.WriteLine("Generate some data first using option 1 in the data management menu.");
                Console.WriteLine();
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"Available datasets for {provider}:");
            for (int i = 0; i < providerDatasets.Count; i++)
            {
                var dataset = providerDatasets[i];
                var status = dataset.IsGenerated ? "Generated" : "Loaded";
                Console.WriteLine($"{i + 1}. {dataset.Name} ({status}, {dataset.CreatedDate:yyyy-MM-dd}, {dataset.RecordCount} records)");
            }
            Console.WriteLine();
            Console.Write("Select the dataset to load: ");

            if (int.TryParse(Console.ReadLine(), out int selection) && selection > 0 && selection <= providerDatasets.Count)
            {
                var selectedDataset = providerDatasets[selection - 1];
                _currentDataset = selectedDataset.Name;
                SaveCurrentDataset();
                
                Console.WriteLine($"Dataset '{selectedDataset.Name}' loaded as current dataset.");
                Console.WriteLine($"Provider: {selectedDataset.Provider}");
                Console.WriteLine($"Records: {selectedDataset.RecordCount}");
            }
            else
            {
                Console.WriteLine("Invalid selection.");
            }

            Console.WriteLine();
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        private static void DeleteOldDatasets()
        {
            Console.WriteLine("=== DELETE OLD DATASETS ===");
            Console.WriteLine();
            
            if (_datasets.Count == 0)
            {
                Console.WriteLine("No datasets to delete.");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Available datasets to delete:");
            for (int i = 0; i < _datasets.Count; i++)
            {
                var dataset = _datasets[i];
                Console.WriteLine($"{i + 1}. {dataset.Name}");
            }
            Console.WriteLine();
            Console.WriteLine("Format: 1,2,3 (to delete multiple) or just 1 (to delete one)");
            Console.Write("Select the datasets to delete: ");

            var input = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("No dataset selected.");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                return;
            }

            try
            {
                var selections = input.Split(',')
                    .Select(s => s.Trim())
                    .Where(s => int.TryParse(s, out _))
                    .Select(int.Parse)
                    .Where(i => i > 0 && i <= _datasets.Count)
                    .OrderByDescending(i => i) // Delete from back to front
                    .ToList();

                if (selections.Count == 0)
                {
                    Console.WriteLine("Invalid selection.");
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                    return;
                }

                foreach (var selection in selections)
                {
                    var dataset = _datasets[selection - 1];
                    
                    if (File.Exists(dataset.FilePath))
                    {
                        File.Delete(dataset.FilePath);
                    }
                    
                    if (_currentDataset == dataset.Name)
                    {
                        _currentDataset = null;
                        SaveCurrentDataset();
                    }
                    
                    _datasets.RemoveAt(selection - 1);
                    Console.WriteLine($"Dataset '{dataset.Name}' deleted.");
                }
                
                SaveDatasetsInfo();
                Console.WriteLine($"{selections.Count} dataset(s) deleted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting datasets: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        private static void ShowExercisesMenu()
        {
            if (string.IsNullOrEmpty(_currentDataset))
            {
                Console.WriteLine("No dataset loaded.");
                Console.WriteLine("Please load a dataset from 'Manage data' first.");
                Console.WriteLine();
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"=== EXECUTE EXERCISES ===");
            Console.WriteLine($"Current dataset: {_currentDataset}");
            Console.WriteLine();
            Console.WriteLine("Select the exercise to execute:");
            Console.WriteLine();
            Console.WriteLine("1. Get the top 50 most ordered products");
            Console.WriteLine("2. Get list of customers with more than 2 addresses");
            Console.WriteLine("3. Get cities of the top 10 products ordered this year");
            Console.WriteLine("4. Get the top 10 customers who make the most orders this year");
            Console.WriteLine("5. Get the top 10 cities where customers are registered");
            Console.WriteLine("6. Return to main menu");
            Console.WriteLine();
            Console.Write("Select an option: ");

            var choice = Console.ReadLine();
            Console.WriteLine();

            try
            {
                var playground = LoadCurrentDataset();
                if (playground == null)
                {
                    Console.WriteLine("Error loading current dataset.");
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                    return;
                }

                switch (choice)
                {
                    case "1":
                        playground.GetTop50ProductsOrdered();
                        break;
                    case "2":
                        playground.GetCustomersWithMoreThan2Addresses();
                        break;
                    case "3":
                        playground.GetCitiesOfTop10ProductsThisYear();
                        break;
                    case "4":
                        playground.GetTop10CustomersMostOrdersThisYear();
                        break;
                    case "5":
                        playground.GetTop10CitiesWhereCustomersAreRegistered();
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing exercise: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        private static Playground? LoadCurrentDataset()
        {
            if (string.IsNullOrEmpty(_currentDataset))
            {
                Console.WriteLine("No current dataset selected.");
                return null;
            }

            var provider = GetCurrentProvider();
            var dataset = _datasets.FirstOrDefault(d => d.Name == _currentDataset && string.Equals((d.Provider ?? "Sqlite").Trim(), provider, StringComparison.OrdinalIgnoreCase));
            if (dataset == null)
            {
                Console.WriteLine($"Dataset '{_currentDataset}' not found for provider '{provider}'.");
                Console.WriteLine($"Available datasets for {provider}: {string.Join(", ", _datasets.Where(d => string.Equals((d.Provider ?? "Sqlite").Trim(), provider, StringComparison.OrdinalIgnoreCase)).Select(d => d.Name))}");
                return null;
            }

            // Check if database file exists for SQLite
            if (provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
            {
                var configuredPath = _configuration?["ConnectionStrings:SqlitePath"] ?? "stark_database.db";
                var sqlitePath = Path.IsPathRooted(configuredPath) ? configuredPath : GetDataFilePath(configuredPath);
                if (!File.Exists(sqlitePath))
                {
                    Console.WriteLine($"SQLite database file not found: {sqlitePath}");
                    return null;
                }
            }

            try
            {
                var adapter = GetAdapter();
                var context = adapter.CreateContext(""); // Use single DB per provider
                
                // Test the connection by counting records
                var customerCount = context.Customers.Count();
                var productCount = context.Products.Count();
                var addressCount = context.Addresses.Count();
                var orderCount = context.Orders.Count();
                var orderItemCount = context.OrderItems.Count();

                Console.WriteLine($"Connected to database with {customerCount} customers, {productCount} products, {addressCount} addresses, {orderCount} orders, {orderItemCount} order items");
                return new Playground(context);
            }
            catch (Exception ex)
            {
                var inner = GetInnermostExceptionMessage(ex);
                Console.WriteLine($"Error loading dataset: {ex.Message}. Inner: {inner}");
                return null;
            }
        }

        private static void SaveToDatabase(StubRecords stubRecords, string identifier, string provider)
        {
            var adapter = GetAdapter();
            // For SQL Server ensure the database exists first
            if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                EnsureSqlServerDatabaseExists();
                CreateSqlServerDatabaseIfNotExists("StarkDb");
            }

            using var context = adapter.CreateContext(""); // Use single DB per provider

            // Ensure the database is clean and created
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    // For SQL Server, create new objects without explicit IDs
                    // The EnsureDeleted/EnsureCreated already cleared everything

                    // ID remapping dictionaries (original -> new SQL ID)
                    var customerIdMap = new Dictionary<int, int>();
                    var productIdMap = new Dictionary<int, int>();
                    var addressIdMap = new Dictionary<int, int>();
                    var orderIdMap = new Dictionary<int, int>();

                    // Insert Customers first (no dependencies)
                    var originalCustomers = (stubRecords.Customers ?? new List<Customer>()).ToList();
                    if (originalCustomers.Any())
                    {
                        var customersForSqlServer = originalCustomers.Select(c => new Customer
                        {
                            FirstName = c.FirstName,
                            LastName = c.LastName,
                            Email = c.Email,
                            CustomerType = c.CustomerType
                        }).ToList();

                        context.Customers.AddRange(customersForSqlServer);
                        context.SaveChanges();

                        for (int i = 0; i < originalCustomers.Count; i++)
                        {
                            var originalId = originalCustomers[i].CustomerId;
                            var newId = customersForSqlServer[i].CustomerId;
                            customerIdMap[originalId] = newId;
                        }
                    }

                    // Insert Products second (no dependencies)
                    var originalProducts = (stubRecords.Products ?? new List<Product>()).ToList();
                    if (originalProducts.Any())
                    {
                        var productsForSqlServer = originalProducts.Select(p => new Product
                        {
                            ProductName = p.ProductName,
                            Description = p.Description,
                            CurrentPrice = p.CurrentPrice
                        }).ToList();

                        context.Products.AddRange(productsForSqlServer);
                        context.SaveChanges();

                        for (int i = 0; i < originalProducts.Count; i++)
                        {
                            var originalId = originalProducts[i].ProductId;
                            var newId = productsForSqlServer[i].ProductId;
                            productIdMap[originalId] = newId;
                        }
                    }

                    // Insert Addresses third (depends on Customers)
                    var originalAddresses = (stubRecords.Addresses ?? new List<Address>()).ToList();
                    if (originalAddresses.Any())
                    {
                        var addressesForSqlServer = originalAddresses.Select(a => new Address
                        {
                            CustomerId = customerIdMap.TryGetValue(a.CustomerId, out var mappedCustomerId) ? mappedCustomerId : a.CustomerId,
                            AddressLine1 = a.AddressLine1,
                            AddressLine2 = a.AddressLine2,
                            City = a.City,
                            State = a.State,
                            Country = a.Country,
                            PostalCode = a.PostalCode,
                            AddressType = a.AddressType
                        }).ToList();

                        context.Addresses.AddRange(addressesForSqlServer);
                        context.SaveChanges();

                        for (int i = 0; i < originalAddresses.Count; i++)
                        {
                            var originalId = originalAddresses[i].AddressId;
                            var newId = addressesForSqlServer[i].AddressId;
                            addressIdMap[originalId] = newId;
                        }
                    }

                    // Insert Orders fourth (depends on Customers and Addresses)
                    var originalOrders = (stubRecords.Orders ?? new List<Order>()).ToList();
                    if (originalOrders.Any())
                    {
                        var ordersForSqlServer = originalOrders.Select(o => new Order
                        {
                            CustomerId = customerIdMap.TryGetValue(o.CustomerId, out var mappedCustomerId) ? mappedCustomerId : o.CustomerId,
                            ShippingAddressId = addressIdMap.TryGetValue(o.ShippingAddressId, out var mappedAddressId) ? mappedAddressId : o.ShippingAddressId,
                            OrderDate = o.OrderDate
                        }).ToList();

                        context.Orders.AddRange(ordersForSqlServer);
                        context.SaveChanges();

                        for (int i = 0; i < originalOrders.Count; i++)
                        {
                            var originalId = originalOrders[i].OrderId;
                            var newId = ordersForSqlServer[i].OrderId;
                            orderIdMap[originalId] = newId;
                        }
                    }

                    // Insert OrderItems last (depends on Orders and Products)
                    if (originalOrders.Any())
                    {
                        var items = originalOrders.SelectMany(o => o.OrderItems ?? new List<Stark.Common.Models.OrderItem>()).ToList();
                        if (items.Any())
                        {
                            var orderItemsForSqlServer = items.Select(oi => new OrderItem(0, orderIdMap.TryGetValue(oi.OrderId, out var mappedOrderId) ? mappedOrderId : oi.OrderId)
                            {
                                ProductId = productIdMap.TryGetValue(oi.ProductId, out var mappedProductId) ? mappedProductId : oi.ProductId,
                                Quantity = oi.Quantity,
                                PurchasePrice = oi.PurchasePrice
                            }).ToList();

                            context.OrderItems.AddRange(orderItemsForSqlServer);
                            context.SaveChanges();
                        }
                    }

                    Console.WriteLine($"Data saved successfully to SQL Server database: StarkDb");
                }
                catch (Exception ex)
                {
                    var inner = GetInnermostExceptionMessage(ex);
                    throw new Exception($"SQL Server save failed. Inner: {inner}");
                }
            }
            else
            {
                // SQLite - Add all data and save
                if (stubRecords.Customers != null && stubRecords.Customers.Any())
                {
                    context.Customers.AddRange(stubRecords.Customers);
                }
                
                if (stubRecords.Products != null && stubRecords.Products.Any())
                {
                    context.Products.AddRange(stubRecords.Products);
                }
                
                if (stubRecords.Addresses != null && stubRecords.Addresses.Any())
                {
                    context.Addresses.AddRange(stubRecords.Addresses);
                }
                
                if (stubRecords.Orders != null && stubRecords.Orders.Any())
                {
                    context.Orders.AddRange(stubRecords.Orders);
                }

                try
                {
                    context.SaveChanges();
                    Console.WriteLine($"Data saved successfully to SQLite database: {_configuration?["ConnectionStrings:SqlitePath"] ?? "stark_database.db"}");
                }
                catch (Exception ex)
                {
                    var inner = GetInnermostExceptionMessage(ex);
                    throw new Exception($"SaveChanges failed. Inner: {inner}");
                }
            }
        }

        private static void EnsureSqlServerDatabaseExists()
        {
            // No-op placeholder to keep symmetry; server existence assumed via connection string
        }

        private static void CreateSqlServerDatabaseIfNotExists(string databaseName)
        {
            var baseCs = _configuration?["ConnectionStrings:SqlServer"] ?? "Server=localhost\\SQLEXPRESS;Database=StarkDb;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True";
            var builder = new SqlConnectionStringBuilder(baseCs);
            var serverCs = baseCs;
            builder.Remove("Initial Catalog");
            builder.Remove("Database");
            serverCs = builder.ConnectionString;

            using var connection = new SqlConnection(serverCs);
            connection.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = $"IF DB_ID(@db) IS NULL CREATE DATABASE [{databaseName}]";
            cmd.Parameters.AddWithValue("@db", databaseName);
            cmd.ExecuteNonQuery();
        }

        private static string GetInnermostExceptionMessage(Exception ex)
        {
            var cur = ex;
            while (cur.InnerException != null) cur = cur.InnerException;
            return cur.Message;
        }

        private static void LoadDatasets()
        {
            try
            {
                var infoPath = GetDataFilePath("datasets_info.json");
                if (File.Exists(infoPath))
                {
                    var json = File.ReadAllText(infoPath);
                    _datasets = JsonSerializer.Deserialize<List<DatasetInfo>>(json) ?? new List<DatasetInfo>();
                }

                var currentPath = GetDataFilePath("current_dataset.txt");
                if (File.Exists(currentPath))
                {
                    _currentDataset = File.ReadAllText(currentPath).Trim();
                }

                // Ensure current dataset belongs to the active provider
                var provider = GetCurrentProvider();
                var belongsToProvider = _datasets.Any(d => d.Name == _currentDataset && string.Equals((d.Provider ?? "Sqlite").Trim(), provider, StringComparison.OrdinalIgnoreCase));
                if (!belongsToProvider)
                {
                    _currentDataset = null;
                    SaveCurrentDataset();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading datasets information: {ex.Message}");
                _datasets = new List<DatasetInfo>();
            }
        }

        private static void SaveDatasetsInfo()
        {
            try
            {
                var json = JsonSerializer.Serialize(
                    _datasets,
                    new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }
                );
                File.WriteAllText(GetDataFilePath("datasets_info.json"), json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving datasets information: {ex.Message}");
            }
        }

        private static void SaveCurrentDataset()
        {
            try
            {
                File.WriteAllText(GetDataFilePath("current_dataset.txt"), _currentDataset ?? "");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving current dataset: {ex.Message}");
            }
        }
    }

    public class DatasetInfo
    {
        public string Name { get; set; } = "";
        public string? Provider { get; set; } = null; // Sqlite or SqlServer
        public string? FilePath { get; set; } = null; // Only for Sqlite
        public string? DatabaseName { get; set; } = null; // Only for SqlServer
        public DateTime CreatedDate { get; set; }
        public int RecordCount { get; set; }
        public bool IsGenerated { get; set; }
    }

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Read configuration so EF Tools target the selected provider
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../")))
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            var provider = configuration["Database:Provider"] ?? "Sqlite";
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                var cs = configuration["ConnectionStrings:SqlServer"] ?? "Server=localhost\\SQLEXPRESS;Database=StarkDb;Trusted_Connection=True;TrustServerCertificate=True";
                optionsBuilder.UseSqlServer(cs);
            }
            else
            {
                var path = configuration["ConnectionStrings:SqlitePath"] ?? "stark_database.db";
                optionsBuilder.UseSqlite($"Data Source={path}");
            }

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
