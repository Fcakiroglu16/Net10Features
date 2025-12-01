using Microsoft.EntityFrameworkCore;
using Net10Features.EFCoreFeatures.Models;

namespace Net10Features.EFCoreFeatures.Features;

/// <summary>
/// Demonstrates EF Core Compiled Queries - A performance optimization feature
/// 
/// WHAT ARE COMPILED QUERIES?
/// Compiled queries are pre-compiled LINQ queries that can be reused multiple times
/// without incurring the overhead of query translation and compilation on each execution.
/// 
/// WHY USE COMPILED QUERIES?
/// - Performance: Query translation is done once and cached
/// - Reduced overhead: Subsequent executions skip the translation phase
/// - Predictable performance: Especially beneficial for frequently executed queries
/// - Memory efficiency: Reduces allocations in the query pipeline
/// 
/// WHEN TO USE:
/// - High-frequency queries (executed many times)
/// - Performance-critical paths
/// - Queries with simple to moderate complexity
/// - Parameterized queries that follow the same pattern
/// 
/// WHEN NOT TO USE:
/// - Rarely executed queries
/// - Dynamic queries with varying structure
/// - One-time queries
/// </summary>
public class CompiledQueries
{
    // ===================================================================
    // BASIC COMPILED QUERY
    // ===================================================================
    // Compiled query that finds a product by ID
    // The query is compiled once and can be reused multiple times
    // 
    // Signature: Func<DbContext, Parameter1, Parameter2, ..., TResult>
    // - First parameter is always the DbContext
    // - Additional parameters are query parameters
    // - Last generic parameter is the return type
    private static readonly Func<AppDbContext, int, Product?> GetProductById =
        EF.CompileQuery((AppDbContext context, int productId) =>
            context.Products.FirstOrDefault(p => p.Id == productId));

    // ===================================================================
    // COMPILED QUERY WITH MULTIPLE PARAMETERS
    // ===================================================================
    // Find products by price range
    // Demonstrates how to use multiple parameters in compiled queries
    private static readonly Func<AppDbContext, decimal, decimal, IEnumerable<Product>> GetProductsByPriceRange =
        EF.CompileQuery((AppDbContext context, decimal minPrice, decimal maxPrice) =>
            context.Products
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .OrderBy(p => p.Price));

    // ===================================================================
    // COMPILED QUERY WITH INCLUDES (EAGER LOADING)
    // ===================================================================
    // Get customer with their orders
    // Shows how to use Include with compiled queries for eager loading
    private static readonly Func<AppDbContext, int, Customer?> GetCustomerWithOrders =
        EF.CompileQuery((AppDbContext context, int customerId) =>
            context.Customers
                .Include(c => c.Orders)
                .FirstOrDefault(c => c.Id == customerId));

    // ===================================================================
    // COMPILED QUERY WITH PROJECTION
    // ===================================================================
    // Get product summary (using Select to return a subset of data)
    // Compiled queries work great with projections for DTOs
    private static readonly Func<AppDbContext, int, ProductSummary?> GetProductSummary =
        EF.CompileQuery((AppDbContext context, int productId) =>
            context.Products
                .Where(p => p.Id == productId)
                .Select(p => new ProductSummary
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    TagCount = p.Tags.Count
                })
                .FirstOrDefault());

    // ===================================================================
    // COMPILED QUERY WITH FILTERING AND ORDERING
    // ===================================================================
    // Search products by name containing a search term
    // Demonstrates string operations in compiled queries
    private static readonly Func<AppDbContext, string, IEnumerable<Product>> SearchProducts =
        EF.CompileQuery((AppDbContext context, string searchTerm) =>
            context.Products
                .Where(p => p.Name.Contains(searchTerm))
                .OrderBy(p => p.Name)
                .Take(10)); // Limit results for performance

    // ===================================================================
    // COMPILED QUERY FOR AGGREGATION
    // ===================================================================
    // Get count of products in a price range
    // Compiled queries can also be used for aggregate operations
    private static readonly Func<AppDbContext, decimal, decimal, int> CountProductsInPriceRange =
        EF.CompileQuery((AppDbContext context, decimal minPrice, decimal maxPrice) =>
            context.Products
                .Count(p => p.Price >= minPrice && p.Price <= maxPrice));

    // ===================================================================
    // COMPILED QUERY WITH COMPLEX FILTERING
    // ===================================================================
    // Get products with specific tags
    // Demonstrates working with primitive collections in compiled queries
    private static readonly Func<AppDbContext, string, IEnumerable<Product>> GetProductsByTag =
        EF.CompileQuery((AppDbContext context, string tag) =>
            context.Products
                .Where(p => p.Tags.Contains(tag))
                .OrderBy(p => p.Name));

    // ===================================================================
    // COMPILED ASYNC QUERY
    // ===================================================================
    // Async version for asynchronous database operations
    // Note: Compiled async queries work with IQueryable operations
    private static readonly Func<AppDbContext, decimal, IQueryable<Product>> GetExpensiveProducts =
        EF.CompileQuery((AppDbContext context, decimal minPrice) =>
            context.Products
                .Where(p => p.Price >= minPrice)
                .OrderByDescending(p => p.Price));

    // ===================================================================
    // COMPILED QUERY WITH SINGLE RESULT
    // ===================================================================
    // Check if a customer exists by email
    // Using Any() for existence checks is more efficient than Count()
    private static readonly Func<AppDbContext, string, bool> CustomerExistsByEmail =
        EF.CompileQuery((AppDbContext context, string email) =>
            context.Customers.Any(c => c.Email == email));

    // ===================================================================
    // DEMONSTRATION METHODS
    // ===================================================================

    public static void DemonstrateCompiledQueries()
    {
        Console.WriteLine("\n=== EF Core Compiled Queries Demo ===\n");

        using var context = new AppDbContext();
        SeedData(context);

        // ---------------------------------------------------------------
        // 1. Basic Compiled Query
        // ---------------------------------------------------------------
        Console.WriteLine("1. Finding product by ID using compiled query:");
        var product = GetProductById(context, 1);
        Console.WriteLine($"   Found: {product?.Name ?? "Not found"}");
        
        // PERFORMANCE NOTE: The first call compiles the query
        // Subsequent calls reuse the compiled query plan
        var product2 = GetProductById(context, 2);
        Console.WriteLine($"   Found: {product2?.Name ?? "Not found"} (reused compiled query)");

        // ---------------------------------------------------------------
        // 2. Compiled Query with Multiple Parameters
        // ---------------------------------------------------------------
        Console.WriteLine("\n2. Finding products by price range:");
        var productsInRange = GetProductsByPriceRange(context, 50, 150).ToList();
        Console.WriteLine($"   Found {productsInRange.Count} products between $50-$150");
        foreach (var p in productsInRange)
        {
            Console.WriteLine($"   - {p.Name}: ${p.Price}");
        }

        // ---------------------------------------------------------------
        // 3. Compiled Query with Eager Loading
        // ---------------------------------------------------------------
        Console.WriteLine("\n3. Getting customer with orders:");
        var customer = GetCustomerWithOrders(context, 1);
        if (customer != null)
        {
            Console.WriteLine($"   Customer: {customer.Name}");
            Console.WriteLine($"   Orders: {customer.Orders.Count}");
        }

        // ---------------------------------------------------------------
        // 4. Compiled Query with Projection
        // ---------------------------------------------------------------
        Console.WriteLine("\n4. Getting product summary (projection):");
        var summary = GetProductSummary(context, 1);
        if (summary != null)
        {
            Console.WriteLine($"   {summary.Name} - ${summary.Price} - {summary.TagCount} tags");
        }

        // ---------------------------------------------------------------
        // 5. Compiled Query with Search
        // ---------------------------------------------------------------
        Console.WriteLine("\n5. Searching products:");
        var searchResults = SearchProducts(context, "Laptop").ToList();
        Console.WriteLine($"   Found {searchResults.Count} products matching 'Laptop'");

        // ---------------------------------------------------------------
        // 6. Compiled Query for Aggregation
        // ---------------------------------------------------------------
        Console.WriteLine("\n6. Counting products in price range:");
        var count = CountProductsInPriceRange(context, 100, 500);
        Console.WriteLine($"   {count} products between $100-$500");

        // ---------------------------------------------------------------
        // 7. Compiled Query with Primitive Collections
        // ---------------------------------------------------------------
        Console.WriteLine("\n7. Finding products by tag:");
        var taggedProducts = GetProductsByTag(context, "electronics").ToList();
        Console.WriteLine($"   Found {taggedProducts.Count} products with 'electronics' tag");

        // ---------------------------------------------------------------
        // 8. Compiled Query - Existence Check
        // ---------------------------------------------------------------
        Console.WriteLine("\n8. Checking customer existence:");
        var exists = CustomerExistsByEmail(context, "john@example.com");
        Console.WriteLine($"   Customer exists: {exists}");

        // ---------------------------------------------------------------
        // 9. Compiled Query with Async Execution
        // ---------------------------------------------------------------
        Console.WriteLine("\n9. Compiled query executed asynchronously:");
        Task.Run(async () =>
        {
            var expensiveProducts = await GetExpensiveProducts(context, 200).ToListAsync();
            foreach (var p in expensiveProducts)
            {
                Console.WriteLine($"   - {p.Name}: ${p.Price}");
            }
        }).Wait();

        // ---------------------------------------------------------------
        // PERFORMANCE COMPARISON
        // ---------------------------------------------------------------
        Console.WriteLine("\n=== Performance Comparison ===");
        PerformanceComparison(context);
    }

    /// <summary>
    /// Demonstrates the performance difference between compiled and regular queries
    /// </summary>
    private static void PerformanceComparison(AppDbContext context)
    {
        const int iterations = 1000;

        // Warm up both queries
        GetProductById(context, 1);
        context.Products.FirstOrDefault(p => p.Id == 1);

        // Test compiled query
        var compiledWatch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            var _ = GetProductById(context, 1);
        }
        compiledWatch.Stop();

        // Test regular query
        var regularWatch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            var _ = context.Products.FirstOrDefault(p => p.Id == 1);
        }
        regularWatch.Stop();

        Console.WriteLine($"Compiled Query: {compiledWatch.ElapsedMilliseconds}ms for {iterations} iterations");
        Console.WriteLine($"Regular Query:  {regularWatch.ElapsedMilliseconds}ms for {iterations} iterations");
        Console.WriteLine($"Performance gain: {((float)regularWatch.ElapsedMilliseconds / compiledWatch.ElapsedMilliseconds):F2}x faster");
    }

    /// <summary>
    /// Seeds sample data for demonstration
    /// </summary>
    private static void SeedData(AppDbContext context)
    {
        if (context.Products.Any()) return;

        var products = new[]
        {
            new Product
            {
                Id = 1,
                Name = "Gaming Laptop",
                Price = 1299.99m,
                Tags = new List<string> { "electronics", "computers", "gaming" },
                Ratings = new List<int> { 5, 4, 5, 4, 5 }
            },
            new Product
            {
                Id = 2,
                Name = "Wireless Mouse",
                Price = 29.99m,
                Tags = new List<string> { "electronics", "accessories" },
                Ratings = new List<int> { 4, 4, 5 }
            },
            new Product
            {
                Id = 3,
                Name = "Mechanical Keyboard",
                Price = 149.99m,
                Tags = new List<string> { "electronics", "accessories", "gaming" },
                Ratings = new List<int> { 5, 5, 4 }
            },
            new Product
            {
                Id = 4,
                Name = "USB-C Hub",
                Price = 49.99m,
                Tags = new List<string> { "electronics", "accessories" },
                Ratings = new List<int> { 4, 3, 4 }
            }
        };

        var customer = new Customer
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com",
            DiscountPercentage = 10,
            Orders = new List<Order>
            {
                new Order { Id = 1, TotalAmount = 1299.99m, OrderDate = DateTimeOffset.UtcNow.AddDays(-5) },
                new Order { Id = 2, TotalAmount = 179.98m, OrderDate = DateTimeOffset.UtcNow.AddDays(-2) }
            }
        };

        context.Products.AddRange(products);
        context.Customers.Add(customer);
        context.SaveChanges();
    }
}

/// <summary>
/// DTO for product summary projection
/// </summary>
public class ProductSummary
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int TagCount { get; set; }
}
