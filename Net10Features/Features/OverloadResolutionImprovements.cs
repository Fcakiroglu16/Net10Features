namespace Net10Features.Features;

/// <summary>
/// Demonstrates overload resolution improvements in C# 14
/// Better type inference and method selection for overloaded methods
/// </summary>
public class OverloadResolutionImprovements
{
    public static void Demonstrate()
    {
        Console.WriteLine("=== Overload Resolution Improvements ===\n");

        // 1. Improved method overload selection with collection expressions
        Console.WriteLine("1. Better overload selection with collection expressions:");
        
        // C# 14 can better infer which overload to call
        int[] arrayData = [1, 2, 3, 4, 5];
        ProcessData(arrayData); // Calls array version
        ProcessData(new List<int> { 1, 2, 3 }); // Calls List<int> version
        
        // 2. Better type inference with generic methods
        Console.WriteLine("\n2. Improved generic type inference:");
        
        // C# 14 better infers types from collection expressions
        var result1 = Transform([1, 2, 3], x => x * 2);
        Console.WriteLine($"   Transformed: {string.Join(", ", result1)}");
        
        // 3. Improved lambda inference
        Console.WriteLine("\n3. Better lambda type inference:");
        
        // Better inference of delegate types
        var processor = GetProcessor();
        var value = processor([1, 2, 3, 4, 5]);
        Console.WriteLine($"   Processed value: {value}");

        // 4. Params collection overload resolution
        Console.WriteLine("\n4. Params collection overload resolution:");
        
        // Compiler chooses the best params overload
        Calculate(1, 2, 3); // Prefers ReadOnlySpan<int> for efficiency
        Calculate([1, 2, 3, 4, 5, 6, 7, 8, 9, 10]); // Also uses ReadOnlySpan
        
        // 5. Better overload resolution with nullable types
        Console.WriteLine("\n5. Nullable type overload resolution:");
        
        int? nullableValue = 42;
        PrintValue(nullableValue); // Better selection between nullable overloads
        PrintValue((int?)null);
        
        // 6. Span vs Array overload resolution
        Console.WriteLine("\n6. Span vs Array overload resolution:");
        
        // C# 14 better chooses between Span and Array overloads
        int[] data = [1, 2, 3, 4, 5];
        ProcessOptimized(data); // Prefers most efficient overload

        Console.WriteLine();
    }

    // Multiple overloads - compiler chooses best match
    private static void ProcessData(Span<int> data)
    {
        Console.WriteLine($"   Processing Span<int> with {data.Length} elements");
    }

    private static void ProcessData(ReadOnlySpan<int> data)
    {
        Console.WriteLine($"   Processing ReadOnlySpan<int> with {data.Length} elements");
    }

    private static void ProcessData(List<int> data)
    {
        Console.WriteLine($"   Processing List<int> with {data.Count} elements");
    }

    private static void ProcessData(int[] data)
    {
        Console.WriteLine($"   Processing int[] with {data.Length} elements");
    }

    // Generic method with improved inference
    private static TResult[] Transform<T, TResult>(T[] source, Func<T, TResult> selector)
    {
        var result = new TResult[source.Length];
        for (int i = 0; i < source.Length; i++)
        {
            result[i] = selector(source[i]);
        }
        return result;
    }

    // Lambda type inference improvements
    private static Func<int[], int> GetProcessor()
    {
        // C# 14 better infers the return type
        return numbers => numbers.Sum();
    }

    // Params overload resolution
    private static void Calculate(params ReadOnlySpan<int> values)
    {
        var sum = 0;
        foreach (var value in values)
            sum += value;
        Console.WriteLine($"   Sum (ReadOnlySpan): {sum}");
    }

    private static void Calculate(params int[] values)
    {
        Console.WriteLine($"   Sum (Array): {values.Sum()}");
    }

    // Nullable type overloads
    private static void PrintValue(int value)
    {
        Console.WriteLine($"   Non-nullable int: {value}");
    }

    private static void PrintValue(int? value)
    {
        Console.WriteLine($"   Nullable int: {value?.ToString() ?? "null"}");
    }

    // Span vs Array overload selection
    private static void ProcessOptimized(ReadOnlySpan<int> data)
    {
        Console.WriteLine($"   Optimized processing (ReadOnlySpan): {data.Length} items");
    }

    private static void ProcessOptimized(int[] data)
    {
        Console.WriteLine($"   Array processing: {data.Length} items");
    }

    // Example: Overload resolution with constraints
    public static void DemonstrateConstraints()
    {
        Console.WriteLine("\n=== Overload Resolution with Constraints ===");

        // Better selection when multiple constraints are involved
        int[] numbers = [1, 2, 3, 4, 5];
        string[] strings = ["a", "b", "c"];

        ProcessComparable(numbers); // Calls IComparable<T> version
        ProcessEquatable(strings);  // Calls IEquatable<T> version
    }

    private static void ProcessComparable<T>(T[] items) where T : IComparable<T>
    {
        Console.WriteLine($"   Processing {items.Length} comparable items");
        if (items.Length > 0)
        {
            var max = items.Max();
            Console.WriteLine($"   Max value: {max}");
        }
    }

    private static void ProcessEquatable<T>(T[] items) where T : IEquatable<T>
    {
        Console.WriteLine($"   Processing {items.Length} equatable items");
    }

    // Example: Method group improvements
    public static void DemonstrateMethodGroups()
    {
        Console.WriteLine("\n=== Method Group Improvements ===");

        int[] numbers = [1, 2, 3, 4, 5];
        
        // Better inference when using method groups
        var doubled = numbers.Select(Double).ToArray();
        Console.WriteLine($"   Doubled: {string.Join(", ", doubled)}");

        // Better overload resolution with delegates
        ProcessWithDelegate(numbers, Double);
    }

    private static int Double(int x) => x * 2;
    
    private static void ProcessWithDelegate(int[] values, Func<int, int> transform)
    {
        var results = values.Select(transform);
        Console.WriteLine($"   Processed: {string.Join(", ", results)}");
    }

    // Example: Extension method overload resolution
    public static void DemonstrateExtensionMethods()
    {
        Console.WriteLine("\n=== Extension Method Overload Resolution ===");

        int[] numbers = [1, 2, 3, 4, 5];
        
        // C# 14 better resolves extension methods vs instance methods
        var result = numbers.CustomWhere(x => x > 2);
        Console.WriteLine($"   Filtered: {string.Join(", ", result)}");
    }
}

// Extension methods for demonstration
public static class OverloadResolutionExtensions
{
    public static IEnumerable<T> CustomWhere<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        foreach (var item in source)
        {
            if (predicate(item))
                yield return item;
        }
    }
}
