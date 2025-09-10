using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Stark.BL;
using Stark.Common.Models;
using Stark.Generators;
using System.Text.Json;

namespace LINQ.Playground
{
    class Program
    {
        private static string? _currentDataset = null;
        private static List<DatasetInfo> _datasets = new List<DatasetInfo>();

        static void Main(string[] args)
        {
            Console.WriteLine("=== STARK CUSTOMER MANAGEMENT SYSTEM ===");
            Console.WriteLine("LINQ demonstration system with Entity Framework Core");
            Console.WriteLine();

            LoadDatasets();
            ShowMainMenu();
        }

        private static void ShowMainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== MAIN MENU ===");
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
                Console.WriteLine("2. Load data from SQLite");
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
                        LoadDataFromSQLite();
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
                Console.WriteLine("🔄 Generating data...");
                
                var stubRecords = Playground.GenerateDataWithFallback(dataCount, dataCount, dataCount);
                
                Console.WriteLine($"✅ Data generated:");
                Console.WriteLine($"   - Customers: {stubRecords.Customers?.Count ?? 0}");
                Console.WriteLine($"   - Products: {stubRecords.Products?.Count ?? 0}");
                Console.WriteLine($"   - Addresses: {stubRecords.Addresses?.Count ?? 0}");
                Console.WriteLine($"   - Orders: {stubRecords.Orders?.Count ?? 0}");
                Console.WriteLine();

                var dbPath = $"datasets/{datasetName.Replace(" ", "_")}.db";
                Directory.CreateDirectory("datasets");
                
                SaveToSQLite(stubRecords, dbPath);
                var datasetInfo = new DatasetInfo
                {
                    Name = datasetName,
                    FilePath = dbPath,
                    CreatedDate = DateTime.Now,
                    RecordCount = (stubRecords.Customers?.Count ?? 0) + 
                                 (stubRecords.Products?.Count ?? 0) + 
                                 (stubRecords.Addresses?.Count ?? 0) + 
                                 (stubRecords.Orders?.Count ?? 0),
                    IsGenerated = true
                };
                
                _datasets.Add(datasetInfo);
                SaveDatasetsInfo();
                
                Console.WriteLine($"Dataset '{datasetName}' saved successfully to {dbPath}");
                Console.WriteLine($"Total records: {datasetInfo.RecordCount}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating data: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        private static void LoadDataFromSQLite()
        {
            Console.WriteLine("=== LOAD DATA FROM SQLITE ===");
            Console.WriteLine();
            
            if (_datasets.Count == 0)
            {
                Console.WriteLine("No datasets available.");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Available datasets:");
            for (int i = 0; i < _datasets.Count; i++)
            {
                var dataset = _datasets[i];
                var status = dataset.IsGenerated ? "Generated" : "Loaded";
                Console.WriteLine($"{i + 1}. {dataset.Name} ({status}, {dataset.CreatedDate:yyyy-MM-dd}, {dataset.RecordCount} records)");
            }
            Console.WriteLine();
            Console.Write("Select the dataset to load: ");

            if (int.TryParse(Console.ReadLine(), out int selection) && selection > 0 && selection <= _datasets.Count)
            {
                var selectedDataset = _datasets[selection - 1];
                _currentDataset = selectedDataset.Name;
                SaveCurrentDataset();
                
                Console.WriteLine($"Dataset '{selectedDataset.Name}' loaded as current dataset.");
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
                return null;

            var dataset = _datasets.FirstOrDefault(d => d.Name == _currentDataset);
            if (dataset == null || !File.Exists(dataset.FilePath))
                return null;

            try
            {
                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlite($"Data Source={dataset.FilePath}")
                    .Options;

                using var context = new AppDbContext(options);
                
                var customers = context.Customers.Include(c => c.AddressList).ToList();
                var products = context.Products.ToList();
                var addresses = context.Addresses.ToList();
                var orders = context.Orders.Include(o => o.OrderItems).ToList();
                var orderItems = context.OrderItems.ToList();

                return new Playground(customers, products, addresses, orders, orderItems);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading dataset: {ex.Message}");
                return null;
            }
        }

        private static void SaveToSQLite(StubRecords stubRecords, string dbPath)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            using var context = new AppDbContext(options);
            
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            if (stubRecords.Customers != null)
            {
                context.Customers.AddRange(stubRecords.Customers);
            }
            
            if (stubRecords.Products != null)
            {
                context.Products.AddRange(stubRecords.Products);
            }
            
            if (stubRecords.Addresses != null)
            {
                context.Addresses.AddRange(stubRecords.Addresses);
            }
            
            if (stubRecords.Orders != null)
            {
                context.Orders.AddRange(stubRecords.Orders);
            }

            context.SaveChanges();
        }

        private static void LoadDatasets()
        {
            try
            {
                if (File.Exists("datasets_info.json"))
                {
                    var json = File.ReadAllText("datasets_info.json");
                    _datasets = JsonSerializer.Deserialize<List<DatasetInfo>>(json) ?? new List<DatasetInfo>();
                }

                if (File.Exists("current_dataset.txt"))
                {
                    _currentDataset = File.ReadAllText("current_dataset.txt").Trim();
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
                var json = JsonSerializer.Serialize(_datasets, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText("datasets_info.json", json);
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
                File.WriteAllText("current_dataset.txt", _currentDataset ?? "");
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
        public string FilePath { get; set; } = "";
        public DateTime CreatedDate { get; set; }
        public int RecordCount { get; set; }
        public bool IsGenerated { get; set; }
    }

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite("Data Source=stark_database.db");
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
