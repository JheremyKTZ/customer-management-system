using Stark.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LINQ.Playground
{
    internal class Playground
    {
        public Playground(
            List<Customer> customers,
            List<Product> products,
            List<Address> addresses,
            List<Order> orders,
            List<OrderItem> orderItems)
        {
            Customers = customers;
            Products = products;
            Addresses = addresses;
            Orders = orders;
            OrderItems = orderItems;
        }

        public List<Customer> Customers { get; }
        public List<Product> Products { get; }
        public List<Address> Addresses { get; }
        public List<Order> Orders { get; }
        public List<OrderItem> OrderItems { get; }

        internal void Run()
        {
            Console.WriteLine("Obtener clientes con nombres que empiecen con B");

            var customersThatStartWithB = Customers
                .Where(c => c.FullName.StartsWith("B"))
                .Select(c => new
                {
                    c.FullName,
                    c.Email,
                    c.CustomerId
                })
                .ToList();

            Console.WriteLine($"Se encontraron {customersThatStartWithB.Count} clientes que empiezan con B");
            foreach (var customer in customersThatStartWithB.Take(5))
            {
                Console.WriteLine($"- {customer.FullName} ({customer.Email})");
            }

            Console.WriteLine("\nObtener los 10 mejores direcciones que contienen una ciudad con más de 1 palabra");

            var addressesLongCountry = Addresses
                .Where(a => a.City.Split(' ').Length > 1)
                .Select(a => new
                {
                    a.AddressId,
                    a.Country,
                    a.City,
                    a.PostalCode,
                    a.AddressLine1,
                    CityWords = a.City.Split(' ').Length,
                })
                .OrderBy(a => a.CityWords)
                .ThenBy(a => a.City)
                .Take(10)
                .ToList();

            Console.WriteLine($"Se encontraron {addressesLongCountry.Count} direcciones con ciudades de múltiples palabras");
            foreach (var address in addressesLongCountry)
            {
                Console.WriteLine($"- {address.City} ({address.CityWords} palabras) - {address.Country}");
            }

            Console.WriteLine("\nObtener los siguientes 10 pedidos que tienen la fecha de pedido más reciente");
            var recentlyOrderedOrders = Orders
                .OrderByDescending(o => o.OrderDate)
                .Skip(10)
                .Take(10)
                .ToList();

            Console.WriteLine($"Se encontraron {recentlyOrderedOrders.Count} pedidos recientes");
            foreach (var order in recentlyOrderedOrders.Take(5))
            {
                Console.WriteLine($"- Pedido {order.OrderId} del {order.OrderDate:yyyy-MM-dd}");
            }

            Console.WriteLine("\nObtener el elemento de pedido superior e inferior, por cantidad y precio");
            var orderedItems = OrderItems
                .OrderByDescending(o => o.Quantity)
                .ThenBy(o => o.PurchasePrice);
            var topOrderItem = orderedItems.First();
            var bottomOrderItem = orderedItems.Last();

            Console.WriteLine($"Elemento con mayor cantidad: {topOrderItem.Quantity} unidades a ${topOrderItem.PurchasePrice}");
            Console.WriteLine($"Elemento con menor cantidad: {bottomOrderItem.Quantity} unidades a ${bottomOrderItem.PurchasePrice}");

            Console.WriteLine("\nObtener información del cliente desde los pedidos");
            var ordersWithCustomers = Orders
                .Join(Customers,
                    o => o.CustomerId,
                    c => c.CustomerId,
                    (o, c) => new
                    {
                        o.OrderId,
                        c.CustomerId,
                        c.FullName,
                        c.Email,
                        o.OrderDate,
                        Items = o.OrderItems.Count
                    })
                .ToList();

            Console.WriteLine($"Se encontraron {ordersWithCustomers.Count} pedidos con información de clientes");
            foreach (var order in ordersWithCustomers.Take(5))
            {
                Console.WriteLine($"- Pedido {order.OrderId} de {order.FullName} ({order.Items} artículos)");
            }
        }
    }
}
