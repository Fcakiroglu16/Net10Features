using Microsoft.EntityFrameworkCore;
using Net10Features.EFCoreFeatures.Models;

namespace Net10Features.EFCoreFeatures.Features;

/// <summary>
/// Demonstrates Primitive Collections in EF Core 10
/// Store collections of primitive types (int, string, etc.) directly in database columns.
/// Previously, you needed separate tables or JSON columns for this.
/// </summary>
public class PrimitiveCollections
{
    public static async Task DemonstrateAsync()
    {
        Console.WriteLine("=== EF Core 10: Primitive Collections ===\n");

        using var context = new AppDbContext();
        await context.Database.EnsureCreatedAsync();

        // 1. Create products with primitive collections
        Console.WriteLine("1. Creating products with primitive collections...");
        
        var products = new[]
        {
            new Product
            {
                Name = "Smartphone",
                Price = 899.99m,
                Tags = ["electronics", "mobile", "5G", "waterproof"],
                Ratings = [5, 5, 4, 5, 5],
                ShippingAddress = new Address { City = "Seattle", Country = "USA", Street = "123 Main", PostalCode = "98101" },
                BillingAddress = new Address { City = "Seattle", Country = "USA", Street = "123 Main", PostalCode = "98101" },
                Metadata = new ProductMetadata { Description = "Latest smartphone" }
            },
            new Product
            {
                Name = "Headphones",
                Price = 299.99m,
                Tags = ["electronics", "audio", "wireless", "noise-cancelling"],
                Ratings = [4, 5, 5, 4],
                ShippingAddress = new Address { City = "Portland", Country = "USA", Street = "456 Oak", PostalCode = "97201" },
                BillingAddress = new Address { City = "Portland", Country = "USA", Street = "456 Oak", PostalCode = "97201" },
                Metadata = new ProductMetadata { Description = "Premium headphones" }
            },
            new Product
            {
                Name = "Tablet",
                Price = 599.99m,
                Tags = ["electronics", "tablet", "portable"],
                Ratings = [5, 4, 4, 5, 4, 5],
                ShippingAddress = new Address { City = "Austin", Country = "USA", Street = "789 Elm", PostalCode = "73301" },
                BillingAddress = new Address { City = "Austin", Country = "USA", Street = "789 Elm", PostalCode = "73301" },
                Metadata = new ProductMetadata { Description = "Lightweight tablet" }
            }
        };

        context.Products.AddRange(products);
        await context.SaveChangesAsync();

        foreach (var p in products)
        {
            Console.WriteLine($"   {p.Name}: {p.Tags.Count} tags, {p.Ratings.Count} ratings");
        }

        // 2. Query by primitive collection elements
        Console.WriteLine("\n2. Querying products that contain specific tags...");
        
        var electronicsProducts = await context.Products
            .Where(p => p.Tags.Contains("electronics"))
            .ToListAsync();

        Console.WriteLine($"   Products with 'electronics' tag: {electronicsProducts.Count}");
        foreach (var p in electronicsProducts)
        {
            Console.WriteLine($"   - {p.Name}");
        }

        // 3. Query by collection count
        Console.WriteLine("\n3. Finding products with many ratings...");
        
        var wellReviewedProducts = await context.Products
            .Where(p => p.Ratings.Count >= 5)
            .ToListAsync();

        Console.WriteLine($"   Products with 5+ ratings: {wellReviewedProducts.Count}");
        foreach (var p in wellReviewedProducts)
        {
            Console.WriteLine($"   - {p.Name}: {p.Ratings.Count} ratings");
        }

        // 4. Calculate average rating using LINQ
        Console.WriteLine("\n4. Calculating average ratings...");
        
        var allProducts = await context.Products.ToListAsync();
        
        foreach (var p in allProducts)
        {
            var avgRating = p.Ratings.Any() ? p.Ratings.Average() : 0;
            Console.WriteLine($"   {p.Name}: {avgRating:F2} average rating");
        }

        // 5. Update primitive collections
        Console.WriteLine("\n5. Adding new tags and ratings...");
        
        var smartphone = await context.Products
            .FirstAsync(p => p.Name == "Smartphone");
        
        smartphone.Tags.Add("bestseller");
        smartphone.Ratings.Add(5);
        
        await context.SaveChangesAsync();
        Console.WriteLine($"   Updated {smartphone.Name}");
        Console.WriteLine($"   New tags: {string.Join(", ", smartphone.Tags)}");
        Console.WriteLine($"   Total ratings: {smartphone.Ratings.Count}");

        // 6. Query with multiple conditions on collections
        Console.WriteLine("\n6. Complex collection queries...");
        
        var premiumProducts = await context.Products
            .Where(p => p.Tags.Contains("electronics") && p.Tags.Contains("wireless"))
            .ToListAsync();

        Console.WriteLine($"   Wireless electronics: {premiumProducts.Count}");
        foreach (var p in premiumProducts)
        {
            Console.WriteLine($"   - {p.Name}: {string.Join(", ", p.Tags)}");
        }

        // Key Benefits of Primitive Collections:
        Console.WriteLine("\n?? Primitive Collections Benefits:");
        Console.WriteLine("   ? No need for separate junction tables");
        Console.WriteLine("   ? Stored efficiently in database (as JSON or array)");
        Console.WriteLine("   ? Can query collection contents with LINQ");
        Console.WriteLine("   ? Supports Contains(), Count, and other operations");
        Console.WriteLine("   ? Simplifies schema for simple collections");
        Console.WriteLine("   ? Better performance than separate tables for small collections");

        Console.WriteLine();
    }
}
