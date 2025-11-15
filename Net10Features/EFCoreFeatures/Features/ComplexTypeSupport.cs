using Microsoft.EntityFrameworkCore;
using Net10Features.EFCoreFeatures.Models;

namespace Net10Features.EFCoreFeatures.Features;

/// <summary>
/// Demonstrates Complex Type Support in EF Core 10
/// Complex types are value objects stored in the same table as the owning entity.
/// They don't have their own identity and are always loaded with the owner.
/// This is different from owned entities which can have separate tables.
/// </summary>
public class ComplexTypeSupport
{
    public static async Task DemonstrateAsync()
    {
        Console.WriteLine("=== EF Core 10: Complex Type Support ===\n");

        using var context = new AppDbContext();
        
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // 1. Create a product with complex types (addresses)
        Console.WriteLine("1. Creating product with complex type addresses...");
        
        var product = new Product
        {
            Name = "Laptop",
            Price = 1299.99m,
            ShippingAddress = new Address
            {
                Street = "123 Main St",
                City = "Seattle",
                PostalCode = "98101",
                Country = "USA"
            },
            BillingAddress = new Address
            {
                Street = "456 Business Ave",
                City = "New York",
                PostalCode = "10001",
                Country = "USA"
            },
            Tags = ["electronics", "computer", "laptop"],
            Ratings = [5, 4, 5, 5],
            Metadata = new ProductMetadata
            {
                Description = "High-performance laptop for professionals",
                Attributes = new Dictionary<string, string>
                {
                    ["Brand"] = "TechCorp",
                    ["Warranty"] = "2 years",
                    ["Color"] = "Silver"
                }
            }
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();
        
        Console.WriteLine($"   Created: {product.Name}");
        Console.WriteLine($"   Shipping: {product.ShippingAddress.City}, {product.ShippingAddress.Country}");
        Console.WriteLine($"   Billing: {product.BillingAddress.City}, {product.BillingAddress.Country}");

        // 2. Query products by complex type properties
        Console.WriteLine("\n2. Querying products by complex type properties...");
        
        var seattleProducts = await context.Products
            .Where(p => p.ShippingAddress.City == "Seattle")
            .ToListAsync();

        Console.WriteLine($"   Found {seattleProducts.Count} product(s) shipping to Seattle");

        // 3. Update complex type
        Console.WriteLine("\n3. Updating complex type (changing shipping address)...");
        
        var productToUpdate = await context.Products.FirstAsync();
        productToUpdate.ShippingAddress.Street = "789 New Address Blvd";
        productToUpdate.ShippingAddress.PostalCode = "98102";
        
        await context.SaveChangesAsync();
        Console.WriteLine($"   Updated shipping address: {productToUpdate.ShippingAddress.Street}");

        // 4. Complex type is always loaded with the entity
        Console.WriteLine("\n4. Complex types are always loaded (no lazy loading)...");
        
        var loadedProduct = await context.Products.FirstAsync();
        Console.WriteLine($"   Product: {loadedProduct.Name}");
        Console.WriteLine($"   Shipping always loaded: {loadedProduct.ShippingAddress.City}");
        Console.WriteLine($"   Billing always loaded: {loadedProduct.BillingAddress.City}");

        // 5. Complex types can be compared
        Console.WriteLine("\n5. Comparing complex types...");
        
        var productsWithSameAddress = await context.Products
            .Where(p => p.ShippingAddress.City == p.BillingAddress.City)
            .ToListAsync();

        Console.WriteLine($"   Products with same shipping and billing city: {productsWithSameAddress.Count}");

        // Key Benefits of Complex Types:
        Console.WriteLine("\n?? Complex Type Benefits:");
        Console.WriteLine("   ? No separate table - stored in same table as owner");
        Console.WriteLine("   ? No identity - value objects, not entities");
        Console.WriteLine("   ? Always loaded with owner - no lazy loading overhead");
        Console.WriteLine("   ? Can be queried and filtered");
        Console.WriteLine("   ? Reduces table joins for better performance");
        Console.WriteLine("   ? Perfect for value objects like Address, Money, etc.");

        Console.WriteLine();
    }
}
