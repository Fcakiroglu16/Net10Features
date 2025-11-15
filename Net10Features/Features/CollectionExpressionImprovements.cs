namespace Net10Features.Features;

/// <summary>
/// Demonstrates collection expression improvements in C# 14 / .NET 10
/// Collection expressions provide a concise syntax for creating collections
/// </summary>
public class CollectionExpressionImprovements
{
    public static void Demonstrate()
    {
        Console.WriteLine("=== Collection Expression Improvements ===\n");

        // 1. Basic collection expressions (C# 12+)
        // Simple syntax to create arrays and lists
        int[] numbers = [1, 2, 3, 4, 5];
        List<string> names = ["Alice", "Bob", "Charlie"];
        
        Console.WriteLine($"Numbers: {string.Join(", ", numbers)}");
        Console.WriteLine($"Names: {string.Join(", ", names)}");

        // 2. Spread operator (..) - Enhanced in C# 14
        // Allows spreading elements from one collection into another
        int[] moreNumbers = [6, 7, 8];
        int[] combined = [..numbers, ..moreNumbers, 9, 10];
        
        Console.WriteLine($"Combined: {string.Join(", ", combined)}");

        // 3. Nested collection expressions
        // Create multidimensional collections easily
        int[][] matrix = [[1, 2, 3], [4, 5, 6], [7, 8, 9]];
        
        Console.WriteLine("\nMatrix:");
        foreach (var row in matrix)
        {
            Console.WriteLine($"  [{string.Join(", ", row)}]");
        }

        // 4. Collection expressions with different types
        // Works with various collection types
        Span<int> span = [1, 2, 3, 4, 5];
        ReadOnlySpan<int> readOnlySpan = [10, 20, 30];
        HashSet<string> uniqueNames = ["Alice", "Bob", "Alice", "Charlie"]; // Duplicates removed
        
        Console.WriteLine($"\nUnique names: {string.Join(", ", uniqueNames)}");

        // 5. Spreading with conditions
        // Conditionally include elements
        var includeExtra = true;
        int[] extra = includeExtra ? [4, 5, 6] : [];
        int[] conditional = [1, 2, 3, ..extra];
        
        Console.WriteLine($"Conditional: {string.Join(", ", conditional)}");

        // 6. Dictionary collection expressions (C# 14 improvement)
        // More concise dictionary initialization
        Dictionary<string, int> ages = new()
        {
            ["Alice"] = 25,
            ["Bob"] = 30,
            ["Charlie"] = 35
        };
        
        Console.WriteLine("\nAges:");
        foreach (var (name, age) in ages)
        {
            Console.WriteLine($"  {name}: {age}");
        }

        Console.WriteLine();
    }

    // Example method using collection expressions in parameters
    public static int Sum(params int[] values) => values.Sum();

    // Example: Using collection expressions with LINQ
    public static void LinqWithCollectionExpressions()
    {
        int[] numbers = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        var evenNumbers = numbers.Where(n => n % 2 == 0).ToArray();
        int[] result = [..evenNumbers, 12, 14, 16];
        
        Console.WriteLine($"Even numbers extended: {string.Join(", ", result)}");
    }
}
