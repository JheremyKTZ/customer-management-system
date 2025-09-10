using Stark.Common.Models;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stark.Playground.Tests
{
    public class SimpleQueryTests
    {
        [Fact]
        public void GetTop50ProductsOrdered_WithMockData_ShouldReturnCorrectResults()
        {
            // Arrange - Create specific mock data
            var mockProducts = CreateMockProducts();
            var mockOrderItems = CreateMockOrderItems(mockProducts);

            // Act - Simulate LINQ query
            var result = mockOrderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    ProductName = mockProducts.FirstOrDefault(p => p.ProductId == g.Key)?.ProductName ?? "Unknown Product",
                    TotalQuantity = g.Sum(oi => oi.Quantity),
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(oi => oi.Quantity * (oi.PurchasePrice ?? 0))
                })
                .OrderByDescending(p => p.TotalQuantity)
                .Take(50)
                .ToList();

            // Assert
            Assert.True(result.Count > 0);
            Assert.True(result.All(p => p.TotalQuantity > 0));
            Assert.True(result.All(p => !string.IsNullOrEmpty(p.ProductName)));
            
            // Verify that it's ordered by quantity
            for (int i = 0; i < result.Count - 1; i++)
            {
                Assert.True(result[i].TotalQuantity >= result[i + 1].TotalQuantity);
            }
        }

        [Fact]
        public void GetCustomersWithMoreThan2Addresses_WithMockData_ShouldReturnCorrectCustomers()
        {
            // Arrange
            var mockCustomers = CreateMockCustomers();

            // Act - Simulate LINQ query
            var result = mockCustomers
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

            // Assert
            Assert.True(result.All(c => c.AddressCount > 2));
            Assert.True(result.All(c => !string.IsNullOrEmpty(c.FullName)));
            Assert.True(result.All(c => !string.IsNullOrEmpty(c.Email)));
            
            // Verify that it's ordered by address count
            for (int i = 0; i < result.Count - 1; i++)
            {
                Assert.True(result[i].AddressCount >= result[i + 1].AddressCount);
            }
        }

        [Fact]
        public void GetCitiesOfTop10ProductsThisYear_WithMockData_ShouldReturnCorrectCities()
        {
            // Arrange
            var mockProducts = CreateMockProducts();
            var mockOrderItems = CreateMockOrderItems(mockProducts);
            var mockAddresses = CreateMockAddresses();
            var mockOrders = CreateMockOrders(mockAddresses);
            
            // Connect OrderItems with Orders
            foreach (var orderItem in mockOrderItems)
            {
                var matchingOrder = mockOrders.FirstOrDefault(o => o.OrderId == orderItem.OrderId);
                if (matchingOrder != null)
                {
                    orderItem.Order = matchingOrder;
                }
            }

            var currentYear = DateTime.Now.Year;

            // Act - Simulate LINQ query
            var top10ProductsThisYear = mockOrderItems
                .Where(oi => oi.Order?.OrderDate?.Year == currentYear)
                .GroupBy(oi => oi.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    ProductName = mockProducts.FirstOrDefault(p => p.ProductId == g.Key)?.ProductName ?? "Unknown Product",
                    TotalQuantity = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(p => p.TotalQuantity)
                .Take(10)
                .ToList();

            var citiesOfTopProducts = top10ProductsThisYear
                .SelectMany(p => mockOrderItems
                    .Where(oi => oi.ProductId == p.ProductId && oi.Order?.OrderDate?.Year == currentYear)
                    .Where(oi => oi.Order?.ShippingAddress != null)
                    .Select(oi => oi.Order.ShippingAddress.City))
                .Where(city => !string.IsNullOrEmpty(city))
                .Distinct()
                .ToList();

            // Assert
            Assert.True(top10ProductsThisYear.Count <= 10);
            Assert.True(citiesOfTopProducts.All(city => !string.IsNullOrEmpty(city)));
        }

        [Fact]
        public void GetTop10CustomersMostOrdersThisYear_WithMockData_ShouldReturnCorrectCustomers()
        {
            // Arrange
            var mockCustomers = CreateMockCustomers();
            var mockOrders = CreateMockOrdersForCustomers(mockCustomers);
            var currentYear = DateTime.Now.Year;

            // Act - Simulate LINQ query
            var result = mockOrders
                .Where(o => o.OrderDate?.Year == currentYear)
                .GroupBy(o => o.CustomerId)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    CustomerName = mockCustomers.FirstOrDefault(c => c.CustomerId == g.Key)?.FullName ?? "Unknown Customer",
                    OrderCount = g.Count(),
                    TotalSpent = g.SelectMany(o => o.OrderItems).Sum(oi => oi.Quantity * (oi.PurchasePrice ?? 0))
                })
                .OrderByDescending(c => c.OrderCount)
                .ThenByDescending(c => c.TotalSpent)
                .Take(10)
                .ToList();

            // Assert
            Assert.True(result.Count <= 10);
            Assert.True(result.All(c => c.OrderCount > 0));
            Assert.True(result.All(c => !string.IsNullOrEmpty(c.CustomerName)));
        }

        [Fact]
        public void GetTop10CitiesWhereCustomersAreRegistered_WithMockData_ShouldReturnCorrectCities()
        {
            // Arrange
            var mockAddresses = CreateMockAddresses();

            // Act - Simulate LINQ query
            var result = mockAddresses
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

            // Assert
            Assert.True(result.Count <= 10);
            Assert.True(result.All(c => !string.IsNullOrEmpty(c.City)));
            Assert.True(result.All(c => c.CustomerCount > 0));
            Assert.True(result.All(c => c.TotalAddresses > 0));
        }

        [Fact]
        public void AllQueries_WithCompleteMockData_ShouldExecuteWithoutErrors()
        {
            // Arrange
            var mockCustomers = CreateMockCustomers();
            var mockProducts = CreateMockProducts();
            var mockAddresses = CreateMockAddresses();
            var mockOrders = CreateMockOrdersForCustomers(mockCustomers);
            var mockOrderItems = CreateMockOrderItems(mockProducts);

            // Act & Assert - Test that all queries execute without errors

            // Query 1: Top 50 products
            var topProducts = mockOrderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    ProductName = mockProducts.FirstOrDefault(p => p.ProductId == g.Key)?.ProductName ?? "Unknown Product",
                    TotalQuantity = g.Sum(oi => oi.Quantity),
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(oi => oi.Quantity * (oi.PurchasePrice ?? 0))
                })
                .OrderByDescending(p => p.TotalQuantity)
                .Take(50)
                .ToList();

            Assert.True(topProducts.Count > 0);

            // Query 2: Customers with more than 2 addresses
            var customersWithManyAddresses = mockCustomers
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

            Assert.True(customersWithManyAddresses.All(c => c.AddressCount > 2));

            // Query 3: Top 10 cities where customers are registered
            var topCities = mockAddresses
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

            Assert.True(topCities.All(c => !string.IsNullOrEmpty(c.City)));

            // Query 4: Top 10 customers with most orders this year
            var currentYear = DateTime.Now.Year;
            var topCustomersThisYear = mockOrders
                .Where(o => o.OrderDate?.Year == currentYear)
                .GroupBy(o => o.CustomerId)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    CustomerName = mockCustomers.FirstOrDefault(c => c.CustomerId == g.Key)?.FullName ?? "Unknown Customer",
                    OrderCount = g.Count(),
                    TotalSpent = g.SelectMany(o => o.OrderItems).Sum(oi => oi.Quantity * (oi.PurchasePrice ?? 0))
                })
                .OrderByDescending(c => c.OrderCount)
                .ThenByDescending(c => c.TotalSpent)
                .Take(10)
                .ToList();

            Assert.True(topCustomersThisYear.All(c => c.OrderCount > 0));

            // Query 5: Cities of top 10 products ordered this year
            var top10ProductsThisYear = mockOrderItems
                .Where(oi => oi.Order?.OrderDate?.Year == currentYear)
                .GroupBy(oi => oi.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    ProductName = mockProducts.FirstOrDefault(p => p.ProductId == g.Key)?.ProductName ?? "Unknown Product",
                    TotalQuantity = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(p => p.TotalQuantity)
                .Take(10)
                .ToList();

            var citiesOfTopProducts = top10ProductsThisYear
                .SelectMany(p => mockOrderItems
                    .Where(oi => oi.ProductId == p.ProductId && oi.Order?.OrderDate?.Year == currentYear)
                    .Where(oi => oi.Order?.ShippingAddress != null)
                    .Select(oi => oi.Order.ShippingAddress.City))
                .Where(city => !string.IsNullOrEmpty(city))
                .Distinct()
                .ToList();

            Assert.True(citiesOfTopProducts.All(city => !string.IsNullOrEmpty(city)));
        }

        private List<Customer> CreateMockCustomers()
        {
            var customers = new List<Customer>();
            
            for (int i = 1; i <= 10; i++)
            {
                var customer = new Customer(i)
                {
                    FirstName = $"Customer{i}",
                    LastName = $"LastName{i}",
                    Email = $"customer{i}@test.com",
                    CustomerType = i % 4
                };
                
                // Add addresses (1-4 per customer)
                int addressCount = (i % 4) + 1;
                for (int j = 1; j <= addressCount; j++)
                {
                    var address = new Address
                    {
                        CustomerId = i,
                        AddressLine1 = $"Street {i}-{j}",
                        City = $"City{i % 5}",
                        Country = j % 2 == 0 ? "Spain" : "Mexico",
                        PostalCode = $"{(i * 1000) + j}",
                        Customer = customer
                    };
                    customer.AddressList.Add(address);
                }
                
                customers.Add(customer);
            }
            
            return customers;
        }

        private List<Product> CreateMockProducts()
        {
            var products = new List<Product>();
            
            for (int i = 1; i <= 15; i++)
            {
                products.Add(new Product
                {
                    ProductName = $"Product{i}",
                    Description = $"Description of product {i}",
                    CurrentPrice = (decimal)(10 + (i * 5.5))
                });
            }
            
            return products;
        }

        private List<Address> CreateMockAddresses()
        {
            var addresses = new List<Address>();
            var cities = new[] { "Madrid", "Barcelona", "Valencia", "Seville", "Bilbao", "Malaga", "Murcia", "Palma", "Las Palmas", "Vitoria" };
            
            for (int i = 1; i <= 20; i++)
            {
                var address = new Address
                {
                    CustomerId = i,
                    AddressLine1 = $"Street {i}",
                    City = cities[i % cities.Length],
                    Country = i % 2 == 0 ? "Spain" : "Mexico",
                    PostalCode = $"{1000 + i}",
                    Customer = new Customer(i)
                };
                addresses.Add(address);
            }
            
            return addresses;
        }

        private List<Order> CreateMockOrders(List<Address> addresses)
        {
            var orders = new List<Order>();
            var random = new Random(42);
            
            for (int i = 1; i <= 20; i++)
            {
                var address = addresses[random.Next(addresses.Count)];
                
                var order = new Order(i)
                {
                    CustomerId = address.CustomerId,
                    ShippingAddressId = address.AddressId,
                    OrderDate = DateTime.Now.AddDays(-random.Next(365)),
                    ShippingAddress = address,
                    OrderItems = new List<OrderItem>()
                };
                
                orders.Add(order);
            }
            
            return orders;
        }

        private List<Order> CreateMockOrdersForCustomers(List<Customer> customers)
        {
            var orders = new List<Order>();
            var random = new Random(42);
            
            for (int i = 1; i <= 20; i++)
            {
                var customer = customers[random.Next(customers.Count)];
                var address = customer.AddressList[random.Next(customer.AddressList.Count)];
                
                var order = new Order(i)
                {
                    CustomerId = customer.CustomerId,
                    ShippingAddressId = address.AddressId,
                    OrderDate = DateTime.Now.AddDays(-random.Next(365)),
                    Customer = customer,
                    ShippingAddress = address,
                    OrderItems = new List<OrderItem>()
                };
                
                orders.Add(order);
            }
            
            return orders;
        }

        private List<OrderItem> CreateMockOrderItems(List<Product> products)
        {
            var orderItems = new List<OrderItem>();
            var random = new Random(42);
            int orderItemId = 1;
            
            for (int orderId = 1; orderId <= 20; orderId++)
            {
                int itemCount = random.Next(1, 4); 
                for (int i = 0; i < itemCount; i++)
                {
                    var product = products[random.Next(products.Count)];
                    var orderItem = new OrderItem(orderItemId, orderId)
                    {
                        ProductId = product.ProductId,
                        Quantity = random.Next(1, 10),
                        PurchasePrice = product.CurrentPrice * (decimal)(0.8 + random.NextDouble() * 0.4),
                        Product = product
                    };
                    
                    orderItems.Add(orderItem);
                    orderItemId++;
                }
            }
            
            return orderItems;
        }
    }
}
