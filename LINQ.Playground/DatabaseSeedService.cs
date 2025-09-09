using Dapper;
using Stark.Common.Models;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace LINQ.Playground
{
    internal static class DatabaseSeedService
    {
        internal static void FillDatabase(
            List<Customer> customers,
            List<Product> products,
            List<Address> addresses,
            List<Order> orders,
            List<OrderItem> orderItems)
        {
            string connectionString = "Data Source=localhost;Initial Catalog=Stark.CustomerManagementSystem.Database;Integrated Security=True";

            string sqlForProducts = "INSERT INTO Products (Id, Name, Description, CurrentPrice)" +
                " VALUES (@ProductId, @ProductName, @Description, @CurrentPrice)";

            using (var connection = new SqlConnection(connectionString))
            {
                // Usar el método Execute de Dapper para insertar múltiples registros
                connection.Execute(sqlForProducts, products);
            }

            string sqlForCustomers = "INSERT INTO Customers (Id, Type, FirstName, LastName, Email)" +
                " VALUES (@CustomerId, @CustomerType, @FirstName, @LastName, @Email)";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Execute(sqlForCustomers, customers);
            }

            string sqlForAddresses = "INSERT INTO Addresses (Id, AddressLine1, AddressLine2, City, State, PostalCode, Country, Type, CustomerId)" +
                " VALUES (@AddressId, @CustomerType, @AddressLine1, @AddressLine2, @City, @State, @PostalCode, @AddressType, @CustomerId)";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Execute(sqlForAddresses, addresses);
            }

            string sqlForOrders = "INSERT INTO Orders (Id, CustomerId, Date, ShippingAddressId)" +
                " VALUES (@OrderId, @CustomerId, @OrderDate, @ShippingAddressId)";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Execute(sqlForOrders, orders);
            }

            string sqlForOrderItems = "INSERT INTO OrderItems (Id, OrderId, ProductId, Quantity, PurchasePrice)" +
                " VALUES (@OrderItemId, @OrderId, @ProductId, @Quantity, @PurchasePrice)";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Execute(sqlForOrderItems, orderItems);
            }
        }
    }
}
