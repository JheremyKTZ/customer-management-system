using Microsoft.EntityFrameworkCore;
using Stark.Common.Models;

namespace Stark.BL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Customer 1:N Address
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.AddressList)
                .WithOne(a => a.Customer)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Customer 1:N Orders
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Address 1:N Orders (Shipping Address)
            modelBuilder.Entity<Address>()
                .HasMany<Order>()
                .WithOne(o => o.ShippingAddress)
                .HasForeignKey(o => o.ShippingAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order 1:N OrderItems
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Product 1:N OrderItems
            modelBuilder.Entity<Product>()
                .HasMany<OrderItem>()
                .WithOne(oi => oi.Product)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Keys
            modelBuilder.Entity<Customer>().HasKey(c => c.CustomerId);
            modelBuilder.Entity<Address>().HasKey(a => a.AddressId);
            modelBuilder.Entity<Product>().HasKey(p => p.ProductId);
            modelBuilder.Entity<Order>().HasKey(o => o.OrderId);
            modelBuilder.Entity<OrderItem>().HasKey(oi => oi.OrderItemId);

            // Strategic Indexes for performance
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .HasDatabaseName("IX_Customers_Email");

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.LastName)
                .HasDatabaseName("IX_Customers_LastName");

            modelBuilder.Entity<Address>()
                .HasIndex(a => a.CustomerId)
                .HasDatabaseName("IX_Addresses_CustomerId");

            modelBuilder.Entity<Address>()
                .HasIndex(a => a.City)
                .HasDatabaseName("IX_Addresses_City");

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.CustomerId)
                .HasDatabaseName("IX_Orders_CustomerId");

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderDate)
                .HasDatabaseName("IX_Orders_OrderDate");

            modelBuilder.Entity<OrderItem>()
                .HasIndex(oi => oi.ProductId)
                .HasDatabaseName("IX_OrderItems_ProductId");

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.ProductName)
                .HasDatabaseName("IX_Products_ProductName");

            // Configurar precisión para decimales (SQLite usa TEXT para decimales)
            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.PurchasePrice)
                .HasConversion<double>();

            modelBuilder.Entity<Product>()
                .Property(p => p.CurrentPrice)
                .HasConversion<double>();

        }
    }
}
