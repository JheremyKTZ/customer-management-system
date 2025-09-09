using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stark.BL;
using Stark.Common.Models;
using System;
using System.Collections.Generic;

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
                    Console.WriteLine("The option doesn't exist; please try again. Select option 1 or 2");
                }
            } while (!success);

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

            Console.WriteLine("Populating the SQLite database");
            PopulateSqliteDatabase();
            return;
        }

        private static void PopulateSqliteDatabase()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite("Data Source=stark_database.db");
            
            using var context = new AppDbContext(optionsBuilder.Options);
            
            context.OrderItems.RemoveRange(context.OrderItems);
            context.Orders.RemoveRange(context.Orders);
            context.Addresses.RemoveRange(context.Addresses);
            context.Products.RemoveRange(context.Products);
            context.Customers.RemoveRange(context.Customers);
            context.SaveChanges();

            // Add new data
            context.Customers.AddRange(_customers);
            context.Products.AddRange(_products);
            context.Addresses.AddRange(_addresses);
            context.Orders.AddRange(_orders);
            context.OrderItems.AddRange(_orderItems);
            
            context.SaveChanges();
            Console.WriteLine("SQLite database populated successfully!");
        }
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
