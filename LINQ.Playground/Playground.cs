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
            Console.WriteLine("Get Customers With Names starting with B");

            var customersThatStartWithB = Customers
                .Where(c => c.FullName.StartsWith("B"))
                .Select(c => new
                {
                    c.FullName,
                    c.Email,
                    c.CustomerId
                })
                .ToList();

            Console.WriteLine("Get Top 10 Addresses that contains a City with more than 1 word");

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

            Console.WriteLine("Get next top 10 Orders that have the most recent order date");
            var recentlyOrderedOrders = Orders
                .OrderByDescending(o => o.OrderDate)
                .Skip(10)
                .Take(10)
                .ToList();

            Console.WriteLine("Get the top and the bottom order items, by quantity and the price");
            var orderedItems = OrderItems
                .OrderByDescending(o => o.Quantity)
                .ThenBy(o => o.PurchasePrice);
            var topOrderItem = orderedItems.First();
            var bottomOrderItem = orderedItems.Last();

            Console.WriteLine("Get Customer Information from Orders");
            var ordersWithCusomers = Orders
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
        }
    }
}
