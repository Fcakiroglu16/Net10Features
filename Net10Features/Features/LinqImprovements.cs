namespace Net10Features.Features;

/// <summary>
/// Demonstrates LINQ improvements in .NET 10
/// New methods like CountBy, AggregateBy, and Index added to LINQ
/// </summary>
public class LinqImprovements
{
    public static void Demonstrate()
    {
        Console.WriteLine("=== LINQ Improvements ===\n");

        // Sample data
        var products = new[]
        {
            new Product("Laptop", "Electronics", 1200),
            new Product("Mouse", "Electronics", 25),
            new Product("Desk", "Furniture", 300),
            new Product("Chair", "Furniture", 150),
            new Product("Monitor", "Electronics", 400),
            new Product("Lamp", "Furniture", 50)
        };

        // 1. CountBy - NEW in .NET 10
        // Counts elements by a key selector, returns dictionary-like result
        Console.WriteLine("1. CountBy - Count products by category:");
        var countByCategory = products.CountBy(p => p.Category);
        foreach (var (category, count) in countByCategory)
        {
            Console.WriteLine($"   {category}: {count} products");
        }

        // 2. AggregateBy - NEW in .NET 10
        // Aggregates elements by a key selector
        Console.WriteLine("\n2. AggregateBy - Sum prices by category:");
        var totalByCategory = products.AggregateBy(
            keySelector: p => p.Category,
            seed: 0m,
            func: (total, product) => total + product.Price
        );
        foreach (var (category, total) in totalByCategory)
        {
            Console.WriteLine($"   {category}: ${total:F2}");
        }

        // 3. Index - NEW in .NET 10
        // Provides index alongside each element
        Console.WriteLine("\n3. Index - Products with their position:");
        foreach (var (index, product) in products.Index())
        {
            Console.WriteLine($"   [{index}] {product.Name} - ${product.Price}");
        }

        // 4. Combining new LINQ features
        Console.WriteLine("\n4. Combining Index with filtering:");
        var expensiveWithIndex = products
            .Where(p => p.Price > 100)
            .Index();
        
        foreach (var (index, product) in expensiveWithIndex)
        {
            Console.WriteLine($"   Position {index}: {product.Name} (${product.Price})");
        }

        // 5. AggregateBy with complex aggregation
        Console.WriteLine("\n5. AggregateBy - Average price by category:");
        var avgByCategory = products
            .GroupBy(p => p.Category)
            .Select(g => new
            {
                Category = g.Key,
                Average = g.Average(p => p.Price),
                Count = g.Count()
            });
        
        foreach (var item in avgByCategory)
        {
            Console.WriteLine($"   {item.Category}: ${item.Average:F2} (from {item.Count} items)");
        }

        // 6. CountBy with complex keys
        Console.WriteLine("\n6. CountBy - Products by price range:");
        var countByPriceRange = products.CountBy(p => p.Price switch
        {
            < 100 => "Budget",
            >= 100 and < 500 => "Mid-range",
            _ => "Premium"
        });
        
        foreach (var item in countByPriceRange)
        {
            Console.WriteLine($"   {item.Key}: {item.Value} products");
        }

        // 7. Multiple aggregations with AggregateBy
        Console.WriteLine("\n7. Multiple statistics by category:");
        DemonstrateComplexAggregation(products);

        Console.WriteLine();
    }

    private static void DemonstrateComplexAggregation(Product[] products)
    {
        // Aggregate to calculate min, max, and average in one pass
        var statsByCategory = products.AggregateBy(
            keySelector: p => p.Category,
            seed: new PriceStats(),
            func: (stats, product) => stats.Add(product.Price)
        );

        foreach (var (category, stats) in statsByCategory)
        {
            Console.WriteLine($"   {category}:");
            Console.WriteLine($"      Min: ${stats.Min:F2}, Max: ${stats.Max:F2}, Avg: ${stats.Average:F2}");
        }
    }

    // Example with custom collections
    public static void DemonstrateWithCustomData()
    {
        Console.WriteLine("\n=== LINQ with Custom Data ===");

        var sales = new[]
        {
            new Sale(DateTime.Now.AddDays(-1), "Alice", 500),
            new Sale(DateTime.Now.AddDays(-2), "Bob", 300),
            new Sale(DateTime.Now.AddDays(-1), "Alice", 200),
            new Sale(DateTime.Now.AddDays(-3), "Charlie", 450),
            new Sale(DateTime.Now.AddDays(-2), "Bob", 150),
        };

        // CountBy to find sales per person
        Console.WriteLine("Sales count by person:");
        var salesPerPerson = sales.CountBy(s => s.SalesPerson);
        foreach (var (person, count) in salesPerPerson)
        {
            Console.WriteLine($"  {person}: {count} sales");
        }

        // AggregateBy to sum revenue per person
        Console.WriteLine("\nTotal revenue by person:");
        var revenuePerPerson = sales.AggregateBy(
            s => s.SalesPerson,
            0m,
            (total, sale) => total + sale.Amount
        );
        foreach (var (person, revenue) in revenuePerPerson)
        {
            Console.WriteLine($"  {person}: ${revenue:F2}");
        }
    }

    // Helper classes
    private record Product(string Name, string Category, decimal Price);
    private record Sale(DateTime Date, string SalesPerson, decimal Amount);

    private class PriceStats
    {
        public decimal Min { get; private set; } = decimal.MaxValue;
        public decimal Max { get; private set; } = decimal.MinValue;
        public decimal Sum { get; private set; }
        public int Count { get; private set; }
        public decimal Average => Count > 0 ? Sum / Count : 0;

        public PriceStats Add(decimal price)
        {
            Min = Math.Min(Min, price);
            Max = Math.Max(Max, price);
            Sum += price;
            Count++;
            return this;
        }
    }
}
