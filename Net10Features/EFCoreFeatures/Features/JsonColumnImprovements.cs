using Microsoft.EntityFrameworkCore;
using Net10Features.EFCoreFeatures.Models;

namespace Net10Features.EFCoreFeatures.Features;

/// <summary>
/// Demonstrates JSON Column improvements in EF Core 10
/// JSON columns allow storing complex objects as JSON in the database.
/// EF Core 10 adds better querying, updating, and indexing support for JSON columns.
/// </summary>
public class JsonColumnImprovements
{
    public static async Task DemonstrateAsync()
    {
        Console.WriteLine("=== EF Core 10: JSON Column Improvements ===\n");

        using var context = new AppDbContext();
        await context.Database.EnsureCreatedAsync();

        // 1. Create products with JSON metadata
        Console.WriteLine("1. Creating products with JSON metadata...");
        
        var products = new[]
        {
            new Product
            {
                Name = "Gaming Laptop",
                Price = 1999.99m,
                ShippingAddress = new Address { City = "Seattle", Country = "USA", Street = "123 Main", PostalCode = "98101" },
                BillingAddress = new Address { City = "Seattle", Country = "USA", Street = "123 Main", PostalCode = "98101" },
                Tags = ["gaming", "laptop"],
                Ratings = [5, 5, 4],
                Metadata = new ProductMetadata
                {
                    Description = "High-end gaming laptop with RGB keyboard",
                    CreatedAt = DateTimeOffset.UtcNow,
                    Attributes = new Dictionary<string, string>
                    {
                        ["Brand"] = "GameTech",
                        ["CPU"] = "Intel i9",
                        ["GPU"] = "RTX 4090",
                        ["RAM"] = "32GB",
                        ["Storage"] = "2TB NVMe SSD",
                        ["Display"] = "17.3\" 4K 144Hz",
                        ["Warranty"] = "3 years"
                    }
                }
            },
            new Product
            {
                Name = "Business Laptop",
                Price = 1299.99m,
                ShippingAddress = new Address { City = "Portland", Country = "USA", Street = "456 Oak", PostalCode = "97201" },
                BillingAddress = new Address { City = "Portland", Country = "USA", Street = "456 Oak", PostalCode = "97201" },
                Tags = ["business", "laptop"],
                Ratings = [4, 5],
                Metadata = new ProductMetadata
                {
                    Description = "Professional laptop for business users",
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-5),
                    Attributes = new Dictionary<string, string>
                    {
                        ["Brand"] = "BizTech",
                        ["CPU"] = "Intel i7",
                        ["RAM"] = "16GB",
                        ["Storage"] = "512GB SSD",
                        ["Display"] = "14\" FHD",
                        ["Warranty"] = "2 years"
                    }
                }
            },
            new Product
            {
                Name = "Ultrabook",
                Price = 899.99m,
                ShippingAddress = new Address { City = "Austin", Country = "USA", Street = "789 Elm", PostalCode = "73301" },
                BillingAddress = new Address { City = "Austin", Country = "USA", Street = "789 Elm", PostalCode = "73301" },
                Tags = ["portable", "laptop"],
                Ratings = [5, 4, 5],
                Metadata = new ProductMetadata
                {
                    Description = "Lightweight ultrabook for travelers",
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-10),
                    Attributes = new Dictionary<string, string>
                    {
                        ["Brand"] = "SlimTech",
                        ["CPU"] = "Intel i5",
                        ["RAM"] = "8GB",
                        ["Storage"] = "256GB SSD",
                        ["Weight"] = "2.2 lbs",
                        ["Warranty"] = "1 year"
                    }
                }
            }
        };

        context.Products.AddRange(products);
        await context.SaveChangesAsync();

        foreach (var p in products)
        {
            Console.WriteLine($"   {p.Name}: {p.Metadata.Attributes.Count} attributes");
        }

        // 2. Query JSON properties (EF Core 10 improvement)
        Console.WriteLine("\n2. Querying products by JSON metadata properties...");
        
        // Query by nested JSON property
        var gamingLaptops = await context.Products
            .Where(p => p.Metadata.Description.Contains("gaming"))
            .ToListAsync();

        Console.WriteLine($"   Gaming laptops found: {gamingLaptops.Count}");
        foreach (var p in gamingLaptops)
        {
            Console.WriteLine($"   - {p.Name}: {p.Metadata.Description}");
        }

        // 3. Query by JSON dictionary values
        Console.WriteLine("\n3. Finding products with specific attributes...");
        
        // In EF Core 10, you can query JSON collections more easily
        var allProductsWithMetadata = await context.Products.ToListAsync();
        
        var intelI9Products = allProductsWithMetadata
            .Where(p => p.Metadata.Attributes.ContainsKey("CPU") && 
                       p.Metadata.Attributes["CPU"].Contains("i9"))
            .ToList();

        Console.WriteLine($"   Products with Intel i9 CPU: {intelI9Products.Count}");
        foreach (var p in intelI9Products)
        {
            Console.WriteLine($"   - {p.Name}: {p.Metadata.Attributes["CPU"]}");
        }

        // 4. Update JSON properties
        Console.WriteLine("\n4. Updating JSON metadata...");
        
        var laptopToUpdate = await context.Products
            .FirstAsync(p => p.Name == "Gaming Laptop");

        // Update nested JSON property
        laptopToUpdate.Metadata.Description = "Premium gaming laptop with latest GPU";
        
        // Update dictionary in JSON
        laptopToUpdate.Metadata.Attributes["GPU"] = "RTX 4090 Ti";
        laptopToUpdate.Metadata.Attributes.Add("Cooling", "Advanced liquid cooling");
        
        await context.SaveChangesAsync();
        
        Console.WriteLine($"   Updated {laptopToUpdate.Name}:");
        Console.WriteLine($"   - New description: {laptopToUpdate.Metadata.Description}");
        Console.WriteLine($"   - GPU: {laptopToUpdate.Metadata.Attributes["GPU"]}");
        Console.WriteLine($"   - Total attributes: {laptopToUpdate.Metadata.Attributes.Count}");

        // 5. Query by JSON date properties
        Console.WriteLine("\n5. Finding recently created products...");
        
        var recentDate = DateTimeOffset.UtcNow.AddDays(-7);
        var recentProducts = await context.Products
            .Where(p => p.Metadata.CreatedAt > recentDate)
            .ToListAsync();

        Console.WriteLine($"   Products created in last 7 days: {recentProducts.Count}");
        foreach (var p in recentProducts)
        {
            Console.WriteLine($"   - {p.Name}: {p.Metadata.CreatedAt:yyyy-MM-dd}");
        }

        // 6. Complex JSON queries
        Console.WriteLine("\n6. Complex queries on JSON data...");
        
        var allProducts = await context.Products.ToListAsync();
        
        var productsWithLongWarranty = allProducts
            .Where(p => p.Metadata.Attributes.ContainsKey("Warranty") &&
                       p.Metadata.Attributes["Warranty"].Contains("3 years"))
            .ToList();

        Console.WriteLine($"   Products with 3+ year warranty: {productsWithLongWarranty.Count}");
        foreach (var p in productsWithLongWarranty)
        {
            Console.WriteLine($"   - {p.Name}: {p.Metadata.Attributes["Warranty"]}");
        }

        // 7. Display full JSON structure
        Console.WriteLine("\n7. Displaying complete JSON metadata...");
        
        var product = await context.Products.FirstAsync();
        Console.WriteLine($"\n   {product.Name} Metadata:");
        Console.WriteLine($"   Description: {product.Metadata.Description}");
        Console.WriteLine($"   Created: {product.Metadata.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"   Attributes:");
        foreach (var (key, value) in product.Metadata.Attributes.OrderBy(x => x.Key))
        {
            Console.WriteLine($"     - {key}: {value}");
        }

        // Key Benefits of JSON Columns:
        Console.WriteLine("\n?? JSON Column Benefits:");
        Console.WriteLine("   ? Store complex, semi-structured data without fixed schema");
        Console.WriteLine("   ? Query nested properties with LINQ");
        Console.WriteLine("   ? Update individual JSON properties efficiently");
        Console.WriteLine("   ? Schema flexibility - add attributes without migrations");
        Console.WriteLine("   ? Better than serializing to string (queryable)");
        Console.WriteLine("   ? Ideal for: metadata, settings, flexible attributes");
        Console.WriteLine("   ? EF Core 10: Improved querying and indexing support");

        Console.WriteLine();
    }
}
