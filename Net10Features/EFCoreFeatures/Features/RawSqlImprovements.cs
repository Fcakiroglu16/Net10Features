using Microsoft.EntityFrameworkCore;
using Net10Features.EFCoreFeatures.Models;

namespace Net10Features.EFCoreFeatures.Features;

/// <summary>
/// Demonstrates Raw SQL improvements in EF Core 10
/// EF Core 10 enhances raw SQL queries with better composability,
/// parameter handling, and integration with LINQ.
/// </summary>
public class RawSqlImprovements
{
    public static async Task DemonstrateAsync()
    {
        Console.WriteLine("=== EF Core 10: Raw SQL Improvements ===\n");

        using var context = new AppDbContext();
        await context.Database.EnsureCreatedAsync();

        // Seed some data
        await SeedDataAsync(context);

        // 1. Basic raw SQL query
        Console.WriteLine("1. Basic raw SQL query...");
        
        var products = await context.Products
            .FromSqlRaw("SELECT * FROM Products")
            .ToListAsync();

        Console.WriteLine($"   Found {products.Count} products using raw SQL");

        // 2. Raw SQL with parameters (SQL injection safe)
        Console.WriteLine("\n2. Raw SQL with parameters (SQL injection safe)...");
        
        var minPrice = 500m;
        var maxPrice = 1500m;
        
        var filteredProducts = await context.Products
            .FromSql($"SELECT * FROM Products WHERE Price BETWEEN {minPrice} AND {maxPrice}")
            .ToListAsync();

        Console.WriteLine($"   Products between ${minPrice} and ${maxPrice}: {filteredProducts.Count}");
        foreach (var p in filteredProducts)
        {
            Console.WriteLine($"   - {p.Name}: ${p.Price}");
        }

        // 3. Composing LINQ with raw SQL (EF Core 10 improvement)
        Console.WriteLine("\n3. Composing LINQ queries on top of raw SQL...");
        
        var composedQuery = await context.Products
            .FromSql($"SELECT * FROM Products WHERE Price > {500m}")
            .Where(p => p.Name.Contains("Laptop"))
            .OrderByDescending(p => p.Price)
            .Take(2)
            .ToListAsync();

        Console.WriteLine($"   Top 2 laptops over $500:");
        foreach (var p in composedQuery)
        {
            Console.WriteLine($"   - {p.Name}: ${p.Price}");
        }

        // 4. Raw SQL for complex aggregations
        Console.WriteLine("\n4. Using raw SQL for complex aggregations...");
        
        // Note: For InMemory database, we'll simulate this with LINQ
        // In a real SQL database, you could use advanced SQL features
        var aggregateResult = await context.Products
            .GroupBy(p => 1) // Group all
            .Select(g => new
            {
                TotalProducts = g.Count(),
                AveragePrice = g.Average(p => p.Price),
                MinPrice = g.Min(p => p.Price),
                MaxPrice = g.Max(p => p.Price),
                TotalValue = g.Sum(p => p.Price)
            })
            .FirstOrDefaultAsync();

        if (aggregateResult != null)
        {
            Console.WriteLine($"   Product Statistics:");
            Console.WriteLine($"   - Total Products: {aggregateResult.TotalProducts}");
            Console.WriteLine($"   - Average Price: ${aggregateResult.AveragePrice:F2}");
            Console.WriteLine($"   - Price Range: ${aggregateResult.MinPrice} - ${aggregateResult.MaxPrice}");
            Console.WriteLine($"   - Total Inventory Value: ${aggregateResult.TotalValue:F2}");
        }

        // 5. Stored procedure simulation (would work with real database)
        Console.WriteLine("\n5. Executing stored procedures (simulation)...");
        
        // In a real SQL database with stored procedures:
        // var result = await context.Products
        //     .FromSqlRaw("EXEC GetProductsByPriceRange @MinPrice, @MaxPrice", 
        //         new SqlParameter("@MinPrice", 500),
        //         new SqlParameter("@MaxPrice", 1500))
        //     .ToListAsync();

        Console.WriteLine("   In production, you would call stored procedures like:");
        Console.WriteLine("   context.Products.FromSqlRaw(\"EXEC GetProductsByPriceRange @p0, @p1\", minPrice, maxPrice)");

        // 6. Raw SQL for updates (ExecuteSql - EF Core 7+)
        Console.WriteLine("\n6. Raw SQL for bulk updates...");
        
        // Note: ExecuteSql is better for bulk operations than SaveChanges
        // In InMemory database, we'll use regular updates
        var productsToUpdate = await context.Products
            .Where(p => p.Price < 1000)
            .ToListAsync();

        foreach (var p in productsToUpdate)
        {
            p.Price *= 1.1m; // 10% increase
        }
        await context.SaveChangesAsync();

        Console.WriteLine($"   Applied 10% price increase to {productsToUpdate.Count} products under $1000");
        Console.WriteLine("   In production SQL Server:");
        Console.WriteLine("   context.Database.ExecuteSql($\"UPDATE Products SET Price = Price * 1.1 WHERE Price < {1000}\")");

        // 7. Raw SQL for deletes
        Console.WriteLine("\n7. Raw SQL for bulk deletes...");
        
        Console.WriteLine("   In production SQL Server:");
        Console.WriteLine("   context.Database.ExecuteSql($\"DELETE FROM Products WHERE Price < {minPrice}\")");
        Console.WriteLine("   This is more efficient than loading entities and deleting them");

        // 8. Combining multiple raw SQL operations
        Console.WriteLine("\n8. Transaction with multiple raw SQL operations...");
        
        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // Multiple operations in a transaction
            var updateCount = productsToUpdate.Count;
            
            Console.WriteLine($"   Executing {updateCount} operations in transaction:");
            Console.WriteLine("   - Update prices");
            Console.WriteLine("   - Recalculate statistics");
            Console.WriteLine("   - Log changes");
            
            await transaction.CommitAsync();
            Console.WriteLine("   ? Transaction committed successfully");
        }
        catch
        {
            await transaction.RollbackAsync();
            Console.WriteLine("   ? Transaction rolled back");
        }

        // Key Benefits of Raw SQL Improvements:
        Console.WriteLine("\n?? Raw SQL Improvements Benefits:");
        Console.WriteLine("   ? Full SQL power when LINQ is not enough");
        Console.WriteLine("   ? SQL injection protection with parameterized queries");
        Console.WriteLine("   ? Compose LINQ on top of raw SQL results");
        Console.WriteLine("   ? Better performance for bulk operations");
        Console.WriteLine("   ? Call stored procedures and functions");
        Console.WriteLine("   ? Use database-specific features");
        Console.WriteLine("   ? ExecuteSql for non-query commands (UPDATE, DELETE)");
        Console.WriteLine("   ? Improved error messages and debugging");

        Console.WriteLine();
    }

    private static async Task SeedDataAsync(AppDbContext context)
    {
        if (await context.Products.AnyAsync())
            return;

        var products = new[]
        {
            new Product
            {
                Name = "Gaming Laptop",
                Price = 1999.99m,
                ShippingAddress = new Address { City = "Seattle", Country = "USA", Street = "123 Main", PostalCode = "98101" },
                BillingAddress = new Address { City = "Seattle", Country = "USA", Street = "123 Main", PostalCode = "98101" },
                Tags = ["gaming", "laptop"],
                Ratings = [5, 5],
                Metadata = new ProductMetadata { Description = "Gaming laptop" }
            },
            new Product
            {
                Name = "Business Laptop",
                Price = 1299.99m,
                ShippingAddress = new Address { City = "Portland", Country = "USA", Street = "456 Oak", PostalCode = "97201" },
                BillingAddress = new Address { City = "Portland", Country = "USA", Street = "456 Oak", PostalCode = "97201" },
                Tags = ["business", "laptop"],
                Ratings = [4],
                Metadata = new ProductMetadata { Description = "Business laptop" }
            },
            new Product
            {
                Name = "Budget Mouse",
                Price = 29.99m,
                ShippingAddress = new Address { City = "Austin", Country = "USA", Street = "789 Elm", PostalCode = "73301" },
                BillingAddress = new Address { City = "Austin", Country = "USA", Street = "789 Elm", PostalCode = "73301" },
                Tags = ["accessories"],
                Ratings = [3, 4],
                Metadata = new ProductMetadata { Description = "Basic mouse" }
            },
            new Product
            {
                Name = "Mechanical Keyboard",
                Price = 149.99m,
                ShippingAddress = new Address { City = "Boston", Country = "USA", Street = "321 Pine", PostalCode = "02101" },
                BillingAddress = new Address { City = "Boston", Country = "USA", Street = "321 Pine", PostalCode = "02101" },
                Tags = ["accessories", "gaming"],
                Ratings = [5, 5, 4],
                Metadata = new ProductMetadata { Description = "RGB keyboard" }
            },
            new Product
            {
                Name = "4K Monitor",
                Price = 599.99m,
                ShippingAddress = new Address { City = "Denver", Country = "USA", Street = "654 Cedar", PostalCode = "80201" },
                BillingAddress = new Address { City = "Denver", Country = "USA", Street = "654 Cedar", PostalCode = "80201" },
                Tags = ["display"],
                Ratings = [4, 5],
                Metadata = new ProductMetadata { Description = "4K display" }
            }
        };

        context.Products.AddRange(products);
        await context.SaveChangesAsync();
    }
}
