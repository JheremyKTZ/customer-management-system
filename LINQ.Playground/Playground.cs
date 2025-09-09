using Stark.Common.Models;

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
            Console.WriteLine("Get customers with names that start with B");

            var customersThatStartWithB = Customers
                .Where(c => c.FullName.StartsWith("B"))
                .Select(c => new
                {
                    c.FullName,
                    c.Email,
                    c.CustomerId
                })
                .ToList();

            Console.WriteLine($"Found {customersThatStartWithB.Count} customers that start with B");
            foreach (var customer in customersThatStartWithB.Take(5))
            {
                Console.WriteLine($"- {customer.FullName} ({customer.Email})");
            }

            Console.WriteLine("\nGet the top 10 addresses that contain a city with more than 1 word");

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

            Console.WriteLine($"Found {addressesLongCountry.Count} addresses with multi-word cities");
            foreach (var address in addressesLongCountry)
            {
                Console.WriteLine($"- {address.City} ({address.CityWords} words) - {address.Country}");
            }

            Console.WriteLine("\nGet the next 10 orders that have the most recent order date");
            var recentlyOrderedOrders = Orders
                .OrderByDescending(o => o.OrderDate)
                .Skip(10)
                .Take(10)
                .ToList();

            Console.WriteLine($"Found {recentlyOrderedOrders.Count} recent orders");
            foreach (var order in recentlyOrderedOrders.Take(5))
            {
                Console.WriteLine($"- Order {order.OrderId} from {order.OrderDate:yyyy-MM-dd}");
            }

            Console.WriteLine("\nGet the top and bottom order item, by quantity and price");
            var orderedItems = OrderItems
                .OrderByDescending(o => o.Quantity)
                .ThenBy(o => o.PurchasePrice);
            var topOrderItem = orderedItems.First();
            var bottomOrderItem = orderedItems.Last();

            Console.WriteLine($"Item with highest quantity: {topOrderItem.Quantity} units at ${topOrderItem.PurchasePrice}");
            Console.WriteLine($"Item with lowest quantity: {bottomOrderItem.Quantity} units at ${bottomOrderItem.PurchasePrice}");

            Console.WriteLine("\nGet customer information from orders");
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

            Console.WriteLine($"Found {ordersWithCustomers.Count} orders with customer information");
            foreach (var order in ordersWithCustomers.Take(5))
            {
                Console.WriteLine($"- Order {order.OrderId} from {order.FullName} ({order.Items} items)");
            }
        }
    }
}
