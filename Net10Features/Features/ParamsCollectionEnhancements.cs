namespace Net10Features.Features;

/// <summary>
/// Demonstrates params collections enhancements in C# 14 / .NET 10
/// params now works with any collection type, not just arrays
/// </summary>
public class ParamsCollectionEnhancements
{
    public static void Demonstrate()
    {
        Console.WriteLine("=== Params Collection Enhancements ===\n");

        // 1. Traditional params with arrays (existing feature)
        Console.WriteLine("Traditional params array:");
        PrintNumbers(1, 2, 3, 4, 5);

        // 2. params with Span<T> - NEW in C# 14
        // More efficient for stack-allocated data
        Console.WriteLine("\nparams with Span<T>:");
        PrintNumbersSpan(10, 20, 30, 40, 50);

        // 3. params with ReadOnlySpan<T> - NEW in C# 14
        // Even more efficient, no copying
        Console.WriteLine("\nparams with ReadOnlySpan<T>:");
        PrintNumbersReadOnlySpan(100, 200, 300);

        // 4. params with List<T> - NEW in C# 14
        Console.WriteLine("\nparams with List<T>:");
        PrintNamesList("Alice", "Bob", "Charlie", "David");

        // 5. params with IEnumerable<T> - NEW in C# 14
        Console.WriteLine("\nparams with IEnumerable<T>:");
        PrintNamesEnumerable("Emma", "Frank", "Grace");

        // 6. Combining params with collection expressions
        Console.WriteLine("\nCombining with collection expressions:");
        int[] numbers = [1, 2, 3];
        PrintNumbers([..numbers, 4, 5, 6]);

        Console.WriteLine();
    }

    // Traditional params with array
    private static void PrintNumbers(params int[] numbers)
    {
        Console.WriteLine($"  Array: {string.Join(", ", numbers)}");
    }

    // NEW: params with Span<T>
    // More efficient - can be stack-allocated
    private static void PrintNumbersSpan(params Span<int> numbers)
    {
        Console.Write("  Span: ");
        foreach (var num in numbers)
        {
            Console.Write($"{num}, ");
        }
        Console.WriteLine();
    }

    // NEW: params with ReadOnlySpan<T>
    // Most efficient - immutable, no copying
    private static void PrintNumbersReadOnlySpan(params ReadOnlySpan<int> numbers)
    {
        Console.Write("  ReadOnlySpan: ");
        foreach (var num in numbers)
        {
            Console.Write($"{num}, ");
        }
        Console.WriteLine();
    }

    // NEW: params with List<T>
    private static void PrintNamesList(params List<string> names)
    {
        Console.WriteLine($"  List: {string.Join(", ", names)}");
    }

    // NEW: params with IEnumerable<T>
    private static void PrintNamesEnumerable(params IEnumerable<string> names)
    {
        Console.WriteLine($"  IEnumerable: {string.Join(", ", names)}");
    }

    // Example: Generic method with params collection
    public static T Max<T>(params ReadOnlySpan<T> values) where T : IComparable<T>
    {
        if (values.IsEmpty)
            throw new ArgumentException("Collection cannot be empty");

        T max = values[0];
        foreach (var value in values)
        {
            if (value.CompareTo(max) > 0)
                max = value;
        }
        return max;
    }

    // Example: params with custom collection type
    public static void PrintHashSet(params HashSet<string> items)
    {
        Console.WriteLine($"HashSet (unique items): {string.Join(", ", items)}");
    }

    // Demonstrate performance benefits
    public static void DemonstratePerformance()
    {
        Console.WriteLine("\n=== Performance Comparison ===");
        
        // With ReadOnlySpan, no heap allocation for small collections
        var sum1 = SumWithSpan(1, 2, 3, 4, 5); // Stack allocated
        Console.WriteLine($"Sum with Span: {sum1}");
        
        // Traditional array params requires heap allocation
        var sum2 = SumWithArray(1, 2, 3, 4, 5); // Heap allocated
        Console.WriteLine($"Sum with Array: {sum2}");
    }

    private static int SumWithSpan(params ReadOnlySpan<int> numbers)
    {
        int sum = 0;
        foreach (var num in numbers)
            sum += num;
        return sum;
    }

    private static int SumWithArray(params int[] numbers)
    {
        return numbers.Sum();
    }
}
