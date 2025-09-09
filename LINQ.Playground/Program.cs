using Stark.Common.Models;

namespace LINQ.Playground
{
    internal class Program
    {
        private static List<Customer> _customers = new List<Customer>();
        private static List<Order> _orders = new List<Order>();
        private static List<OrderItem> _orderItems = new List<OrderItem>();
        private static List<Address> _addresses = new List<Address>();
        private static List<Product> _products = new List<Product>();

        static void Main(string[] _)
        {
            Console.WriteLine("Welcome to the LINQ playground");
            Console.WriteLine("Select 1 to generate new data.");
            Console.WriteLine("Select 2 to use saved data.");
            bool success;
            int numberSelected;
            do
            {
                var optionInput = Console.ReadLine()?.ToString().Trim() ?? "";
                success = int.TryParse(optionInput, out int number);
                numberSelected = number;
                if (!success || numberSelected == 0 || numberSelected > 2)
                {
                    success = false;
                    Console.WriteLine("The option does not exist; please try again. Select option 1 or 2");
                }
            } while (!success);

            // TODO: Validate if the path is valid, use try catch 
            Console.WriteLine("Enter the CSV file path:");
            string filePath = Console.ReadLine() ?? "";
            if (numberSelected == 1)
            {
                (_customers, _products, _addresses, _orders, _orderItems) =
                    FileService.WriteGeneratedDataToCsv(filePath);
                PopulateDatabase();
                Console.WriteLine("Information generated successfully, enjoy your practice session.");
            }
            else
            {
                (_customers, _products, _addresses, _orders, _orderItems) =
                FileService.ReadGeneratedDataFromCsv(filePath);
                PopulateDatabase();
                Console.WriteLine("Information retrieved successfully, enjoy your practice session.");
            }

            Console.WriteLine("Press Enter to continue");
            Console.ReadLine();
            Console.WriteLine("----------------------------------------------------------");
            var playground = new Playground(_customers, _products, _addresses, _orders, _orderItems);
            playground.Run();
            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
        }

        private static void PopulateDatabase()
        {
            Console.WriteLine("Do you want to populate your database with the generated information?");
            Console.WriteLine("1: Yes, 2: No");
            var input = Console.ReadLine()?.Trim() ?? "";
            var success = int.TryParse(input, out int number);
            if (!success || number <= 0 || number >= 3)
            {
                return;
            }

            if (number == 2)
            {
                return;
            }

            Console.WriteLine("Populating the database");
            DatabaseSeedService.FillDatabase(_customers, _products, _addresses, _orders, _orderItems);
            return;
        }
    }
}
