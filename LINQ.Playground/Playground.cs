using Microsoft.EntityFrameworkCore;
using Stark.Common.Models;
using Stark.Generators;
using Stark.BL;

namespace LINQ.Playground
{
    public class Playground
    {
        private readonly AppDbContext _context;

        public Playground(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public static StubRecords GenerateDataWithFallback(int customerCount = 100, int productCount = 50, int orderCount = 200)
        {
            try
            {
                Console.WriteLine("Attempting to generate data using DataBuilder...");
                
                var stubRecords = DataBuilder.Create()
                    .GenerateCustomersAndAddresses(customerCount)
                    .GenerateProducts(productCount)
                    .GenerateOrders(orderCount)
                    .GenerateStubs();

                Console.WriteLine("Data generated successfully with DataBuilder");
                return stubRecords;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating data with DataBuilder: {ex.Message}");
                Console.WriteLine("Generating manual test data...");
                
                return GenerateManualStubData(customerCount, productCount, orderCount);
            }
        }

        private static StubRecords GenerateManualStubData(int customerCount, int productCount, int orderCount)
        {
            var customers = new List<Customer>();
            var addresses = new List<Address>();
            var products = new List<Product>();
            var orders = new List<Order>();
            var orderItems = new List<OrderItem>();

            for (int i = 1; i <= customerCount; i++)
            {
                var customer = new Customer(i)
                {
                    FirstName = $"Customer{i}",
                    LastName = $"LastName{i}",
                    Email = $"customer{i}@email.com",
                    CustomerType = i % 4
                };

                int addressCount = (i % 3) + 1;
                for (int j = 1; j <= addressCount; j++)
                {
                    var address = new Address
                    {
                        CustomerId = i,
                        AddressLine1 = $"Street {i}-{j}",
                        City = $"City {i % 20}",
                        Country = j % 2 == 0 ? "Spain" : "Mexico",
                        PostalCode = $"{(i * 1000) + j}",
                        Customer = customer
                    };
                    addresses.Add(address);
                    customer.AddressList.Add(address);
                }

                customers.Add(customer);
            }

            for (int i = 1; i <= productCount; i++)
            {
                products.Add(new Product
                {
                    ProductName = $"Product {i}",
                    Description = $"Description of product {i}",
                    CurrentPrice = (decimal)(10 + (i * 5.5))
                });
            }

            var random = new Random(42); 
            for (int i = 1; i <= orderCount; i++)
            {
                var customer = customers[random.Next(customers.Count)];
                var address = customer.AddressList[random.Next(customer.AddressList.Count)];
                
                var order = new Order(i)
                {
                    CustomerId = customer.CustomerId,
                    ShippingAddressId = address.AddressId,
                    OrderDate = DateTime.Now.AddDays(-random.Next(365)),
                    Customer = customer,
                    ShippingAddress = address
                };

                orders.Add(order);
            }

            int orderItemId = 1;
            foreach (var order in orders)
            {
                int itemCount = random.Next(1, 6); 
                for (int i = 0; i < itemCount; i++)
                {
                    var product = products[random.Next(products.Count)];
                    var orderItem = new OrderItem(orderItemId, order.OrderId)
                    {
                        ProductId = product.ProductId,
                        Quantity = random.Next(1, 10),
                        PurchasePrice = product.CurrentPrice * (decimal)(0.8 + random.NextDouble() * 0.4), // ±20% of price
                        Order = order,
                        Product = product
                    };
                    orderItems.Add(orderItem);
                    order.OrderItems.Add(orderItem);
                    orderItemId++;
                }
            }

            Console.WriteLine("Manual test data generated successfully");
            return new StubRecords
            {
                Customers = customers,
                Addresses = addresses,
                Products = products,
                Orders = orders
            };
        }

        public void GetTop50ProductsOrdered()
        {
            Console.WriteLine("=== TOP 50 MOST ORDERED PRODUCTS ===");
            Console.WriteLine();

            var topProducts = _context.OrderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    ProductName = _context.Products
                        .Where(p => p.ProductId == g.Key)
                        .Select(p => p.ProductName)
                        .FirstOrDefault() ?? "Unknown Product",
                    TotalQuantity = g.Sum(oi => oi.Quantity),
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(oi => oi.Quantity * (oi.PurchasePrice ?? 0))
                })
                .OrderByDescending(p => p.TotalQuantity)
                .Take(50)
                .ToList();

            Console.WriteLine($"Found {topProducts.Count} products");
            Console.WriteLine();
            Console.WriteLine("Ranking | Product  | Quantity | Orders  | Revenue");
            Console.WriteLine("--------|----------|----------|---------|----------");
            
            for (int i = 0; i < topProducts.Count; i++)
            {
                var product = topProducts[i];
                Console.WriteLine($"{i + 1,7} | {product.ProductName,-8} | {product.TotalQuantity,8} | {product.TotalOrders,7} | ${product.TotalRevenue,8:F2}");
            }
        }

        public void GetCustomersWithMoreThan2Addresses()
        {
            Console.WriteLine("=== CUSTOMERS WITH MORE THAN 2 ADDRESSES ===");
            Console.WriteLine();

            var customersWithManyAddresses = _context.Customers
                .Include(c => c.AddressList)
                .Where(c => c.AddressList.Count > 2)
                .Select(c => new
                {
                    c.CustomerId,
                    c.FullName,
                    c.Email,
                    AddressCount = c.AddressList.Count,
                    Cities = c.AddressList.Select(a => a.City).Distinct().ToList()
                })
                .OrderByDescending(c => c.AddressCount)
                .ToList();

            Console.WriteLine($"Found {customersWithManyAddresses.Count} customers with more than 2 addresses");
            Console.WriteLine();
            Console.WriteLine("ID | Customer | Email | Addresses | Cities");
            Console.WriteLine("---|----------|-------|-----------|--------");
            
            foreach (var customer in customersWithManyAddresses)
            {
                var citiesStr = string.Join(", ", customer.Cities);
                Console.WriteLine($"{customer.CustomerId,2} | {customer.FullName,-8} | {customer.Email,-15} | {customer.AddressCount,1} | {citiesStr}");
            }
        }

        public void GetCitiesOfTop10ProductsThisYear()
        {
            Console.WriteLine("=== CITIES OF TOP 10 PRODUCTS ORDERED THIS YEAR ===");
            Console.WriteLine();

            var currentYear = DateTime.Now.Year;
            var top10ProductsThisYear = _context.OrderItems
                .Include(oi => oi.Order)
                .Where(oi => oi.Order.OrderDate.HasValue && oi.Order.OrderDate.Value.Year == currentYear)
                .GroupBy(oi => oi.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    ProductName = _context.Products
                        .Where(p => p.ProductId == g.Key)
                        .Select(p => p.ProductName)
                        .FirstOrDefault() ?? "Unknown Product",
                    TotalQuantity = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(p => p.TotalQuantity)
                .Take(10)
                .ToList();

            var productIds = top10ProductsThisYear.Select(p => p.ProductId).ToList();
            var citiesOfTopProducts = _context.OrderItems
                .Include(oi => oi.Order)
                .ThenInclude(o => o.ShippingAddress)
                .Where(oi => productIds.Contains(oi.ProductId) && oi.Order.OrderDate.HasValue && oi.Order.OrderDate.Value.Year == currentYear)
                .Where(oi => oi.Order.ShippingAddress != null)
                .Select(oi => oi.Order.ShippingAddress.City)
                .Where(city => !string.IsNullOrEmpty(city))
                .Distinct()
                .ToList();

            Console.WriteLine($"Top 10 products this year ({currentYear}):");
            Console.WriteLine();
            Console.WriteLine("Ranking | Product  | Quantity");
            Console.WriteLine("--------|----------|----------");
            
            for (int i = 0; i < top10ProductsThisYear.Count; i++)
            {
                var product = top10ProductsThisYear[i];
                Console.WriteLine($"{i + 1,7} | {product.ProductName,-8} | {product.TotalQuantity,8}");
            }

            Console.WriteLine();
            Console.WriteLine($"Cities where these products were ordered ({citiesOfTopProducts.Count} cities):");
            Console.WriteLine();
            
            for (int i = 0; i < citiesOfTopProducts.Count; i++)
            {
                Console.WriteLine($"{i + 1,2}. {citiesOfTopProducts[i]}");
            }
        }

        public void GetTop10CustomersMostOrdersThisYear()
        {
            Console.WriteLine("=== TOP 10 CUSTOMERS WITH MOST ORDERS THIS YEAR ===");
            Console.WriteLine();

            var currentYear = DateTime.Now.Year;
            var topCustomersThisYear = _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == currentYear)
                .GroupBy(o => o.CustomerId)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    CustomerName = _context.Customers
                        .Where(c => c.CustomerId == g.Key)
                        .Select(c => c.FullName)
                        .FirstOrDefault() ?? "Unknown Customer",
                    OrderCount = g.Count(),
                    TotalSpent = g.SelectMany(o => o.OrderItems).Sum(oi => oi.Quantity * (oi.PurchasePrice ?? 0))
                })
                .OrderByDescending(c => c.OrderCount)
                .ThenByDescending(c => c.TotalSpent)
                .Take(10)
                .ToList();

            Console.WriteLine($"Top 10 customers with most orders in {currentYear}:");
            Console.WriteLine();
            Console.WriteLine("Ranking | Customer | Orders  | Total Spent");
            Console.WriteLine("--------|----------|---------|-------------");
            
            for (int i = 0; i < topCustomersThisYear.Count; i++)
            {
                var customer = topCustomersThisYear[i];
                Console.WriteLine($"{i + 1,7} | {customer.CustomerName,-8} | {customer.OrderCount,1} | ${customer.TotalSpent,10:F2}");
            }
        }

        public void GetTop10CitiesWhereCustomersAreRegistered()
        {
            Console.WriteLine("=== TOP 10 CITIES WHERE CUSTOMERS ARE REGISTERED ===");
            Console.WriteLine();

            var topCities = _context.Addresses
                .Where(a => !string.IsNullOrEmpty(a.City))
                .GroupBy(a => a.City)
                .Select(g => new
                {
                    City = g.Key,
                    CustomerCount = g.Select(a => a.CustomerId).Distinct().Count(),
                    TotalAddresses = g.Count(),
                    Countries = g.Select(a => a.Country).Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList()
                })
                .OrderByDescending(c => c.CustomerCount)
                .ThenByDescending(c => c.TotalAddresses)
                .Take(10)
                .ToList();

            Console.WriteLine($"Top 10 cities with most registered customers:");
            Console.WriteLine();
            Console.WriteLine("Ranking | City     | Customers | Addresses | Countries");
            Console.WriteLine("--------|----------|-----------|-----------|----------");
            
            for (int i = 0; i < topCities.Count; i++)
            {
                var city = topCities[i];
                var countriesStr = string.Join(", ", city.Countries);
                Console.WriteLine($"{i + 1,7} | {city.City,-8} | {city.CustomerCount,2} | {city.TotalAddresses,2} | {countriesStr}");
            }
        }

        internal void Run()
        {
            Console.WriteLine("=== ADVANCED LINQ QUERIES ===\n");

            GetTop50ProductsOrdered();
            GetCustomersWithMoreThan2Addresses();
            GetCitiesOfTop10ProductsThisYear();
            GetTop10CustomersMostOrdersThisYear();
            GetTop10CitiesWhereCustomersAreRegistered();

            Console.WriteLine("\n=== ORIGINAL QUERIES ===\n");
            RunOriginalQueries();
        }

        private void RunOriginalQueries()
        {
            Console.WriteLine("Get customers with names that start with B");

            var customersThatStartWithB = _context.Customers
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

            var addressesLongCountry = _context.Addresses
                .Where(a => !string.IsNullOrEmpty(a.City))
                .Select(a => new
                {
                    a.AddressId,
                    a.Country,
                    a.City,
                    a.PostalCode,
                    a.AddressLine1,
                })
                .AsEnumerable()
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
            var recentlyOrderedOrders = _context.Orders
                .Where(o => o.OrderDate.HasValue)
                .OrderByDescending(o => o.OrderDate)
                .Skip(10)
                .Take(10)
                .ToList();

            Console.WriteLine($"Found {recentlyOrderedOrders.Count} recent orders");
            foreach (var order in recentlyOrderedOrders.Take(5))
            {
                Console.WriteLine($"- Order {order.OrderId} from {order.OrderDate?.ToString("yyyy-MM-dd")}");
            }

            Console.WriteLine("\nGet the highest and lowest order item, by quantity and price");
            var orderedItems = _context.OrderItems
                .OrderByDescending(o => o.Quantity)
                .ThenBy(o => o.PurchasePrice);
            var topOrderItem = orderedItems.First();
            var bottomOrderItem = orderedItems.Last();

            Console.WriteLine($"Item with highest quantity: {topOrderItem.Quantity} units at ${topOrderItem.PurchasePrice}");
            Console.WriteLine($"Item with lowest quantity: {bottomOrderItem.Quantity} units at ${bottomOrderItem.PurchasePrice}");

            Console.WriteLine("\nGet customer information from orders");
            var ordersWithCustomers = _context.Orders
                .Include(o => o.OrderItems)
                .Join(_context.Customers,
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
