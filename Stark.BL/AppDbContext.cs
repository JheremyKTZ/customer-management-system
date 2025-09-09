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
