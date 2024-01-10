using CsvHelper;
using CsvHelper.Configuration;
using Stark.Common.Models;
using Stark.Generators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace LINQ.Playground
{
    internal static class FileService
    {

        internal static (List<Customer>, List<Product>, List<Address>, List<Order>, List<OrderItem>)
            ReadGeneratedDataFromCsv(string filePath)
        {
            bool readEnabled = false;
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MemberTypes = MemberTypes.Properties,
                IncludePrivateMembers = true,
            };
            List<Customer> customers = new List<Customer>();
            List<Product> products = new List<Product>();
            List<Address> addresses = new List<Address>();
            List<Order> orders = new List<Order>();
            List<OrderItem> orderItems = new List<OrderItem>();

            do
            {
                try
                {
                    using (var reader = new StreamReader($"{filePath}/starkCustomers.csv"))
                    using (var csv = new CsvReader(reader, config))
                    {
                        customers = csv.GetRecords<Customer>().ToList();
                    }

                    using (var reader = new StreamReader($"{filePath}/starkProducts.csv"))
                    using (var csv = new CsvReader(reader, config))
                    {
                        products = csv.GetRecords<Product>().ToList();
                    }

                    using (var reader = new StreamReader($"{filePath}/starkAddresses.csv"))
                    using (var csv = new CsvReader(reader, config))
                    {
                        addresses = csv.GetRecords<Address>().ToList();
                    }

                    using (var reader = new StreamReader($"{filePath}/starkOrders.csv"))
                    using (var csv = new CsvReader(reader, config))
                    {
                        orders = csv.GetRecords<Order>().ToList();
                    }
                    readEnabled = true;

                    using (var reader = new StreamReader($"{filePath}/starkOrderItems.csv"))
                    using (var csv = new CsvReader(reader, config))
                    {
                        orderItems = csv.GetRecords<OrderItem>().ToList();
                    }
                    readEnabled = true;
                }
                catch (Exception)
                {
                    WriteGeneratedDataToCsv(filePath);
                }
            } while (!readEnabled);

            return (customers, products, addresses, orders, orderItems);
        }

        internal static (List<Customer>, List<Product>, List<Address>, List<Order>, List<OrderItem>)
            WriteGeneratedDataToCsv(string filePath)
        {
            var data = DataBuilder.Create()
                               .GenerateCustomersAndAddresses(60)
                               .GenerateProducts(75)
                               .GenerateOrders(62)
                               .GenerateStubs();
            var orderItems = data.Orders.SelectMany(o => o.OrderItems).ToList();

            using (var writer = new StreamWriter($"{filePath}/starkCustomers.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(data.Customers);
            }

            using (var writer = new StreamWriter($"{filePath}/starkProducts.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(data.Products);
            }

            using (var writer = new StreamWriter($"{filePath}/starkAddresses.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(data.Addresses);
            }

            using (var writer = new StreamWriter($"{filePath}/starkOrders.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(data.Orders);
            }

            using (var writer = new StreamWriter($"{filePath}/starkOrderItems.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(orderItems);
            }

            return (
                data.Customers.ToList(),
                data.Products.ToList(),
                data.Addresses.ToList(),
                data.Orders.ToList(),
                orderItems);
        }
    }
}