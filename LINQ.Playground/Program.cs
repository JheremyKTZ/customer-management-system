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
        // Listas para almacenar los datos del sistema
        private static List<Customer> _customers = new List<Customer>();
        private static List<Order> _orders = new List<Order>();
        private static List<OrderItem> _orderItems = new List<OrderItem>();
        private static List<Address> _addresses = new List<Address>();
        private static List<Product> _products = new List<Product>();

        static void Main(string[] _)
        {
            Console.WriteLine("Bienvenido al playground de LINQ");
            Console.WriteLine("Selecciona 1 para generar nuevos datos.");
            Console.WriteLine("Selecciona 2 para usar datos guardados.");
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
                    Console.WriteLine("La opción no existe; por favor intenta de nuevo. Selecciona opción 1 o 2");
                }
            } while (!success);

            // TODO: Validar si la ruta es válida, use try catch 
            Console.WriteLine("Ingresa la ruta del archivo CSV:");
            string filePath = Console.ReadLine() ?? "";
            if (numberSelected == 1)
            {
                (_customers, _products, _addresses, _orders, _orderItems) =
                    FileService.WriteGeneratedDataToCsv(filePath);
                PopulateDatabase();
                Console.WriteLine("Información generada correctamente, disfruta tu sesión de práctica.");
            }
            else
            {
                (_customers, _products, _addresses, _orders, _orderItems) =
                FileService.ReadGeneratedDataFromCsv(filePath);
                PopulateDatabase();
                Console.WriteLine("Información recuperada correctamente, disfruta tu sesión de práctica.");
            }
            
            Console.WriteLine("Presiona Enter para continuar");
            Console.ReadLine();
            Console.WriteLine("----------------------------------------------------------");
            var playground = new Playground(_customers, _products, _addresses, _orders, _orderItems);
            playground.Run();
            Console.WriteLine("Presiona Enter para salir");
            Console.ReadLine();
        }

        // Método para poblar la base de datos con los datos generados
        private static void PopulateDatabase()
        {
            Console.WriteLine("¿Quieres poblar tu base de datos con la información generada?");
            Console.WriteLine("1: Sí, 2: No");
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

            Console.WriteLine("Poblando la base de datos SQLite");
            PopulateSqliteDatabase();
            return;
        }

        private static void PopulateSqliteDatabase()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite("Data Source=stark_database.db");
            
            using var context = new AppDbContext(optionsBuilder.Options);
            
            // Limpiar datos existentes
            context.OrderItems.RemoveRange(context.OrderItems);
            context.Orders.RemoveRange(context.Orders);
            context.Addresses.RemoveRange(context.Addresses);
            context.Products.RemoveRange(context.Products);
            context.Customers.RemoveRange(context.Customers);
            context.SaveChanges();

            // Agregar nuevos datos
            context.Customers.AddRange(_customers);
            context.Products.AddRange(_products);
            context.Addresses.AddRange(_addresses);
            context.Orders.AddRange(_orders);
            context.OrderItems.AddRange(_orderItems);
            
            context.SaveChanges();
            Console.WriteLine("Base de datos SQLite poblada exitosamente!");
        }
    }

    // Clase para configurar EF Core en tiempo de diseño
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
