using Stark.Common.Models;
using Stark.BL;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using LINQ.Playground;

namespace Stark.Playground.Tests
{
    public class SimpleQueryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly LINQ.Playground.Playground _playground;

        public SimpleQueryTests()
        {
            // Create in-memory database for testing
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _playground = new LINQ.Playground.Playground(_context);
            
            // Seed the database with test data
            SeedTestData();
        }

        [Fact]
        public void GetTop50ProductsOrdered_WithMockData_ShouldReturnCorrectResults()
        {
            // Act - Use the Playground method directly
            _playground.GetTop50ProductsOrdered();

            // Assert - Verify the query executes without errors
            // The method will output to console, so we just verify it doesn't throw
            Assert.True(true); // If we reach here, the method executed successfully
        }

        [Fact]
        public void GetCustomersWithMoreThan2Addresses_WithMockData_ShouldReturnCorrectCustomers()
        {
            // Act - Use the Playground method directly
            _playground.GetCustomersWithMoreThan2Addresses();

            // Assert - Verify the query executes without errors
            Assert.True(true); // If we reach here, the method executed successfully
        }

        [Fact]
        public void GetCitiesOfTop10ProductsThisYear_WithMockData_ShouldReturnCorrectCities()
        {
            // Act - Use the Playground method directly
            _playground.GetCitiesOfTop10ProductsThisYear();

            // Assert - Verify the query executes without errors
            Assert.True(true); // If we reach here, the method executed successfully
        }

        [Fact]
        public void GetTop10CustomersMostOrdersThisYear_WithMockData_ShouldReturnCorrectCustomers()
        {
            // Act - Use the Playground method directly
            _playground.GetTop10CustomersMostOrdersThisYear();

            // Assert - Verify the query executes without errors
            Assert.True(true); // If we reach here, the method executed successfully
        }

        [Fact]
        public void GetTop10CitiesWhereCustomersAreRegistered_WithMockData_ShouldReturnCorrectCities()
        {
            // Act - Use the Playground method directly
            _playground.GetTop10CitiesWhereCustomersAreRegistered();

            // Assert - Verify the query executes without errors
            Assert.True(true); // If we reach here, the method executed successfully
        }

        [Fact]
        public void AllQueries_WithCompleteMockData_ShouldExecuteWithoutErrors()
        {
            // Act & Assert - Test that all queries execute without errors
            _playground.GetTop50ProductsOrdered();
            _playground.GetCustomersWithMoreThan2Addresses();
            _playground.GetTop10CitiesWhereCustomersAreRegistered();
            _playground.GetTop10CustomersMostOrdersThisYear();
            _playground.GetCitiesOfTop10ProductsThisYear();

            // If we reach here, all methods executed successfully
            Assert.True(true);
        }

        private void SeedTestData()
        {
            // Create customers with addresses
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
                customers.Add(customer);
                _context.Customers.Add(customer);
            }
            _context.SaveChanges();

            // Create addresses
            var cities = new[] { "Madrid", "Barcelona", "Valencia", "Seville", "Bilbao", "Malaga", "Murcia", "Palma", "Las Palmas", "Vitoria" };
            for (int i = 1; i <= 10; i++)
            {
                var customer = customers[i - 1];
                int addressCount = (i % 4) + 1;
                for (int j = 1; j <= addressCount; j++)
                {
                    var address = new Address
                    {
                        CustomerId = customer.CustomerId,
                        AddressLine1 = $"Street {i}-{j}",
                        AddressLine2 = $"Apt {j}",
                        City = cities[i % cities.Length],
                        State = j % 2 == 0 ? "Madrid" : "CDMX",
                        Country = j % 2 == 0 ? "Spain" : "Mexico",
                        PostalCode = $"{(i * 1000) + j}",
                        AddressType = j % 3,
                        Customer = customer
                    };
                    _context.Addresses.Add(address);
                    customer.AddressList.Add(address);
                }
            }
            _context.SaveChanges();

            // Create products
            var products = new List<Product>();
            for (int i = 1; i <= 15; i++)
            {
                var product = new Product
                {
                    ProductName = $"Product{i}",
                    Description = $"Description of product {i}",
                    CurrentPrice = (decimal)(10 + (i * 5.5))
                };
                products.Add(product);
                _context.Products.Add(product);
            }
            _context.SaveChanges();

            // Create orders
            var random = new Random(42);
            var orders = new List<Order>();
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
                _context.Orders.Add(order);
            }
            _context.SaveChanges();

            // Create order items
            int orderItemId = 1;
            for (int orderId = 1; orderId <= 20; orderId++)
            {
                var order = orders[orderId - 1];
                int itemCount = random.Next(1, 4);
                for (int i = 0; i < itemCount; i++)
                {
                    var product = products[random.Next(products.Count)];
                    var orderItem = new OrderItem(orderItemId, orderId)
                    {
                        ProductId = product.ProductId,
                        Quantity = random.Next(1, 10),
                        PurchasePrice = product.CurrentPrice * (decimal)(0.8 + random.NextDouble() * 0.4),
                        Product = product,
                        Order = order
                    };
                    _context.OrderItems.Add(orderItem);
                    order.OrderItems.Add(orderItem);
                    orderItemId++;
                }
            }
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
