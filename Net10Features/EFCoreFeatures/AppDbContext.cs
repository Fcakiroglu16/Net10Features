using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Net10Features.EFCoreFeatures.Models;

namespace Net10Features.EFCoreFeatures;

/// <summary>
/// Database context demonstrating EF Core 10 features
/// </summary>
public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Using InMemory database for demonstration
        // In production, you would use SQL Server, PostgreSQL, etc.
        optionsBuilder.UseInMemoryDatabase("EFCore10Demo");
        
        // Enable sensitive data logging for demonstration
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ===== Complex Type Configuration (EF Core 10 Feature) =====
        // Complex types are stored in the same table as the owning entity
        // They don't have their own identity and are always loaded with the owner
        modelBuilder.Entity<Product>()
            .ComplexProperty(p => p.ShippingAddress)
            .IsRequired();

        modelBuilder.Entity<Product>()
            .ComplexProperty(p => p.BillingAddress)
            .IsRequired();

        // ===== Primitive Collections Configuration (EF Core 10 Feature) =====
        // Primitive collections are stored as JSON or in separate tables
        modelBuilder.Entity<Product>()
            .Property(p => p.Tags)
            .HasColumnType("nvarchar(max)"); // Stored as JSON in SQL Server

        modelBuilder.Entity<Product>()
            .Property(p => p.Ratings)
            .HasColumnType("nvarchar(max)"); // Stored as JSON in SQL Server

        // ===== JSON Column Configuration =====
        // Store complex objects as JSON columns
        modelBuilder.Entity<Product>()
            .OwnsOne(p => p.Metadata, navigationBuilder =>
            {
                navigationBuilder.ToJson(); // Store as JSON column
            });

        // ===== HierarchyId Configuration (EF Core 10 Feature) =====
        // Configure hierarchical data support
        modelBuilder.Entity<Category>()
            .HasOne(c => c.Parent)
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Category>()
            .HasIndex(c => c.HierarchyPath)
            .IsUnique();

        // ===== Sentinel Values Configuration (EF Core 10 Feature) =====
        // Configure sentinel values to distinguish between "not set" and "set to default"
        modelBuilder.Entity<Customer>()
            .Property(c => c.DiscountPercentage)
            .HasSentinel(-1); // -1 indicates "not set"

        // ===== Relationship Configuration =====
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}
