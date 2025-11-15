namespace Net10Features.Net9Features.Features;

/// <summary>
/// Demonstrates LINQ improvements in .NET 9
/// New methods: CountBy, AggregateBy, and Index provide better performance
/// and more expressive code for common operations.
/// </summary>
public class LinqImprovements
{
    public static void Demonstrate()
    {
        Console.WriteLine("=== .NET 9: LINQ Improvements ===\n");

        // Sample data
        var orders = new[]
        {
            new Order { CustomerId = 1, CustomerName = "Alice", Amount = 100, Category = "Electronics" },
            new Order { CustomerId = 2, CustomerName = "Bob", Amount = 150, Category = "Books" },
            new Order { CustomerId = 1, CustomerName = "Alice", Amount = 200, Category = "Electronics" },
            new Order { CustomerId = 3, CustomerName = "Charlie", Amount = 75, Category = "Books" },
            new Order { CustomerId = 2, CustomerName = "Bob", Amount = 120, Category = "Clothing" },
            new Order { CustomerId = 1, CustomerName = "Alice", Amount = 90, Category = "Books" },
            new Order { CustomerId = 3, CustomerName = "Charlie", Amount = 300, Category = "Electronics" },
        };

        // 1. CountBy - Count occurrences by key
        Console.WriteLine("1. CountBy() - Count orders by customer\n");
        
        // Before .NET 9:
        var countBefore = orders
            .GroupBy(o => o.CustomerName)
            .Select(g => new { Customer = g.Key, Count = g.Count() })
            .ToList();

        // .NET 9:
        var countAfter = orders
            .CountBy(o => o.CustomerName)
            .ToList();

        Console.WriteLine("   Order counts by customer:");
        foreach (var (customer, count) in countAfter)
        {
            Console.WriteLine($"   - {customer}: {count} orders");
        }

        // 2. CountBy with categories
        Console.WriteLine("\n2. CountBy() - Orders by category\n");
        
        var categoryCount = orders
            .CountBy(o => o.Category)
            .OrderByDescending(x => x.Value)
            .ToList();

        Console.WriteLine("   Orders by category:");
        foreach (var (category, count) in categoryCount)
        {
            Console.WriteLine($"   - {category}: {count} orders");
        }

        // 3. AggregateBy - Sum amounts by customer
        Console.WriteLine("\n3. AggregateBy() - Total amount by customer\n");
        
        // Before .NET 9:
        var sumBefore = orders
            .GroupBy(o => o.CustomerName)
            .Select(g => new { Customer = g.Key, Total = g.Sum(o => o.Amount) })
            .ToList();

        // .NET 9:
        var sumAfter = orders
            .AggregateBy(
                keySelector: o => o.CustomerName,
                seed: 0m,
                func: (total, order) => total + order.Amount)
            .ToList();

        Console.WriteLine("   Total spending by customer:");
        foreach (var (customer, total) in sumAfter)
        {
            Console.WriteLine($"   - {customer}: ${total}");
        }

        // 4. AggregateBy - Complex aggregation
        Console.WriteLine("\n4. AggregateBy() - Complex statistics by category\n");
        
        var categoryStats = orders
            .AggregateBy(
                keySelector: o => o.Category,
                seed: new { Count = 0, Total = 0m, Min = decimal.MaxValue, Max = 0m },
                func: (stats, order) => new
                {
                    Count = stats.Count + 1,
                    Total = stats.Total + order.Amount,
                    Min = Math.Min(stats.Min, order.Amount),
                    Max = Math.Max(stats.Max, order.Amount)
                })
            .ToList();

        Console.WriteLine("   Category statistics:");
        foreach (var (category, stats) in categoryStats)
        {
            var avg = stats.Total / stats.Count;
            Console.WriteLine($"   {category}:");
            Console.WriteLine($"     Count: {stats.Count}");
            Console.WriteLine($"     Total: ${stats.Total}");
            Console.WriteLine($"     Average: ${avg:F2}");
            Console.WriteLine($"     Range: ${stats.Min} - ${stats.Max}\n");
        }

        // 5. Index() - Get element with its index
        Console.WriteLine("5. Index() - Enumerate with indices\n");
        
        var products = new[] { "Laptop", "Mouse", "Keyboard", "Monitor", "Headphones" };

        // Before .NET 9:
        Console.WriteLine("   Before .NET 9 (using Select with index):");
        foreach (var (product, index) in products.Select((p, i) => (p, i)))
        {
            Console.WriteLine($"   [{index}] {product}");
        }

        Console.WriteLine("\n   .NET 9 (using Index()):");
        foreach (var (index, product) in products.Index())
        {
            Console.WriteLine($"   [{index}] {product}");
        }

        // 6. Index() with filtering
        Console.WriteLine("\n6. Index() with LINQ operations\n");
        
        var scores = new[] { 85, 92, 78, 95, 88, 76, 90, 82 };
        
        var highScores = scores
            .Index()
            .Where(x => x.Item > 90)
            .ToList();

        Console.WriteLine("   High scores (>90) with their positions:");
        foreach (var (index, score) in highScores)
        {
            Console.WriteLine($"   Position {index}: {score}");
        }

        // 7. Combining new LINQ methods
        Console.WriteLine("\n7. Combining CountBy and Index\n");
        
        var words = new[] { "apple", "banana", "apricot", "blueberry", "avocado", "cherry", "blackberry" };
        
        var letterCounts = words
            .Index()
            .CountBy(x => x.Item[0]) // Count by first letter
            .OrderByDescending(x => x.Value)
            .ToList();

        Console.WriteLine("   Words starting with each letter:");
        foreach (var (letter, count) in letterCounts)
        {
            Console.WriteLine($"   {letter}: {count} word(s)");
        }

        // 8. Performance comparison
        Console.WriteLine("\n8. Performance Benefits\n");
        
        var largeDataset = Enumerable.Range(1, 10000)
            .Select(i => new Order 
            { 
                CustomerId = i % 100, 
                CustomerName = $"Customer{i % 100}",
                Amount = i, 
                Category = i % 3 == 0 ? "A" : i % 3 == 1 ? "B" : "C" 
            })
            .ToArray();

        var sw = System.Diagnostics.Stopwatch.StartNew();
        var oldWay = largeDataset
            .GroupBy(o => o.Category)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .ToList();
        sw.Stop();
        var oldTime = sw.ElapsedMilliseconds;

        sw.Restart();
        var newWay = largeDataset
            .CountBy(o => o.Category)
            .ToList();
        sw.Stop();
        var newTime = sw.ElapsedMilliseconds;

        Console.WriteLine($"   GroupBy + Select: {oldTime}ms");
        Console.WriteLine($"   CountBy: {newTime}ms");
        Console.WriteLine($"   Improvement: {(oldTime - newTime > 0 ? "Faster" : "Similar")}");

        // Key Benefits
        Console.WriteLine("\n?? LINQ Improvements Benefits:");
        Console.WriteLine("   ? CountBy() - Simpler and faster counting by key");
        Console.WriteLine("   ? AggregateBy() - Efficient aggregation without GroupBy");
        Console.WriteLine("   ? Index() - Cleaner enumeration with indices");
        Console.WriteLine("   ? Less allocations - Better memory efficiency");
        Console.WriteLine("   ? More readable - Express intent clearly");
        Console.WriteLine("   ? Better performance - Optimized implementations");

        Console.WriteLine();
    }

    private record Order
    {
        public int CustomerId { get; init; }
        public string CustomerName { get; init; } = string.Empty;
        public decimal Amount { get; init; }
        public string Category { get; init; } = string.Empty;
    }
}
