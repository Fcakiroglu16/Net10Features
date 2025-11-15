using Microsoft.EntityFrameworkCore;
using Net10Features.EFCoreFeatures.Models;

namespace Net10Features.EFCoreFeatures.Features;

/// <summary>
/// Demonstrates HierarchyId support in EF Core 10
/// HierarchyId efficiently represents hierarchical data (tree structures).
/// Common use cases: organizational charts, category trees, file systems, etc.
/// </summary>
public class HierarchyIdSupport
{
    public static async Task DemonstrateAsync()
    {
        Console.WriteLine("=== EF Core 10: HierarchyId Support ===\n");

        using var context = new AppDbContext();
        await context.Database.EnsureCreatedAsync();

        // 1. Create hierarchical category structure
        Console.WriteLine("1. Creating hierarchical category tree...");
        
        // Root categories
        var electronics = new Category 
        { 
            Name = "Electronics", 
            HierarchyPath = "/1/" 
        };
        
        var clothing = new Category 
        { 
            Name = "Clothing", 
            HierarchyPath = "/2/" 
        };

        context.Categories.AddRange(electronics, clothing);
        await context.SaveChangesAsync();

        // Child categories under Electronics
        var computers = new Category 
        { 
            Name = "Computers", 
            ParentId = electronics.Id,
            HierarchyPath = "/1/1/" 
        };
        
        var phones = new Category 
        { 
            Name = "Phones", 
            ParentId = electronics.Id,
            HierarchyPath = "/1/2/" 
        };

        context.Categories.AddRange(computers, phones);
        await context.SaveChangesAsync();

        // Deeper level - Computers subcategories
        var laptops = new Category 
        { 
            Name = "Laptops", 
            ParentId = computers.Id,
            HierarchyPath = "/1/1/1/" 
        };
        
        var desktops = new Category 
        { 
            Name = "Desktops", 
            ParentId = computers.Id,
            HierarchyPath = "/1/1/2/" 
        };

        context.Categories.AddRange(laptops, desktops);
        await context.SaveChangesAsync();

        Console.WriteLine("   Created category hierarchy:");
        Console.WriteLine("   Electronics (/1/)");
        Console.WriteLine("   ??? Computers (/1/1/)");
        Console.WriteLine("   ?   ??? Laptops (/1/1/1/)");
        Console.WriteLine("   ?   ??? Desktops (/1/1/2/)");
        Console.WriteLine("   ??? Phones (/1/2/)");
        Console.WriteLine("   Clothing (/2/)");

        // 2. Find all descendants of a category
        Console.WriteLine("\n2. Finding all descendants of Electronics...");
        
        var electronicsDescendants = await context.Categories
            .Where(c => c.HierarchyPath.StartsWith("/1/") && c.HierarchyPath != "/1/")
            .ToListAsync();

        Console.WriteLine($"   Found {electronicsDescendants.Count} descendants:");
        foreach (var cat in electronicsDescendants.OrderBy(c => c.HierarchyPath))
        {
            var level = cat.HierarchyPath.Split('/', StringSplitOptions.RemoveEmptyEntries).Length - 1;
            var indent = new string(' ', level * 3);
            Console.WriteLine($"   {indent}??? {cat.Name} ({cat.HierarchyPath})");
        }

        // 3. Find all ancestors of a category
        Console.WriteLine("\n3. Finding all ancestors of Laptops...");
        
        var laptopCategory = await context.Categories
            .FirstAsync(c => c.Name == "Laptops");

        // Get all path segments to find ancestors
        var pathSegments = laptopCategory.HierarchyPath
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Select((_, index) => "/" + string.Join("/", laptopCategory.HierarchyPath
                .Split('/', StringSplitOptions.RemoveEmptyEntries)
                .Take(index + 1)) + "/")
            .ToList();

        var ancestors = await context.Categories
            .Where(c => pathSegments.Contains(c.HierarchyPath) && c.Id != laptopCategory.Id)
            .OrderBy(c => c.HierarchyPath)
            .ToListAsync();

        Console.WriteLine($"   Laptop ancestors:");
        foreach (var cat in ancestors)
        {
            Console.WriteLine($"   - {cat.Name} ({cat.HierarchyPath})");
        }

        // 4. Get direct children only
        Console.WriteLine("\n4. Getting direct children of Computers...");
        
        var computersCategory = await context.Categories
            .Include(c => c.Children)
            .FirstAsync(c => c.Name == "Computers");

        Console.WriteLine($"   Direct children of {computersCategory.Name}:");
        foreach (var child in computersCategory.Children)
        {
            Console.WriteLine($"   - {child.Name}");
        }

        // 5. Get category level (depth in tree)
        Console.WriteLine("\n5. Calculating category levels...");
        
        var allCategories = await context.Categories
            .OrderBy(c => c.HierarchyPath)
            .ToListAsync();

        foreach (var cat in allCategories)
        {
            var level = cat.HierarchyPath.Split('/', StringSplitOptions.RemoveEmptyEntries).Length;
            Console.WriteLine($"   Level {level}: {cat.Name} ({cat.HierarchyPath})");
        }

        // 6. Check if category is a descendant of another
        Console.WriteLine("\n6. Checking descendant relationships...");
        
        var isLaptopDescendantOfElectronics = laptopCategory.HierarchyPath.StartsWith(electronics.HierarchyPath);
        var isLaptopDescendantOfClothing = laptopCategory.HierarchyPath.StartsWith(clothing.HierarchyPath);

        Console.WriteLine($"   Is Laptops descendant of Electronics? {isLaptopDescendantOfElectronics}");
        Console.WriteLine($"   Is Laptops descendant of Clothing? {isLaptopDescendantOfClothing}");

        // 7. Find siblings (same parent)
        Console.WriteLine("\n7. Finding siblings of Laptops...");
        
        var laptopSiblings = await context.Categories
            .Where(c => c.ParentId == laptopCategory.ParentId && c.Id != laptopCategory.Id)
            .ToListAsync();

        Console.WriteLine($"   Siblings of Laptops:");
        foreach (var sibling in laptopSiblings)
        {
            Console.WriteLine($"   - {sibling.Name}");
        }

        // Key Benefits of HierarchyId:
        Console.WriteLine("\n?? HierarchyId Benefits:");
        Console.WriteLine("   ? Efficient storage of tree structures");
        Console.WriteLine("   ? Fast queries for ancestors, descendants, and siblings");
        Console.WriteLine("   ? Simple path-based representation (/1/2/3/)");
        Console.WriteLine("   ? Easy to determine relationships (StartsWith, Contains)");
        Console.WriteLine("   ? Supports unlimited depth");
        Console.WriteLine("   ? Better than recursive CTEs for many scenarios");
        Console.WriteLine("   ? Ideal for: org charts, categories, file systems, comment threads");

        Console.WriteLine();
    }
}
