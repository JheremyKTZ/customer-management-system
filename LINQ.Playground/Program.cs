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

            // TODO: Validar si la ruta es válida, usar try catch 
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

            Console.WriteLine("Poblando la base de datos");
            DatabaseSeedService.FillDatabase(_customers, _products, _addresses, _orders, _orderItems);
            return;
        }
    }
}
