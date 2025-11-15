using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Net10Features.Net9Features.Features;

/// <summary>
/// Demonstrates CollectionsMarshal improvements in .NET 9
/// CollectionsMarshal provides low-level, high-performance access to collections.
/// Use carefully - these are advanced, unsafe operations for performance-critical code.
/// </summary>
public class CollectionsMarshalFeatures
{
    public static void Demonstrate()
    {
        Console.WriteLine("=== .NET 9: CollectionsMarshal Features ===\n");

        // 1. AsSpan for List<T> - Direct memory access
        Console.WriteLine("1. AsSpan() for List<T> - Direct Memory Access\n");
        
        var numbers = new List<int> { 1, 2, 3, 4, 5 };
        
        Console.WriteLine($"   Original list: {string.Join(", ", numbers)}");
        
        // Get a span over the list's internal array
        var span = CollectionsMarshal.AsSpan(numbers);
        
        // Modify through span (modifies the list directly)
        for (int i = 0; i < span.Length; i++)
        {
            span[i] *= 2;
        }
        
        Console.WriteLine($"   After doubling via span: {string.Join(", ", numbers)}");
        Console.WriteLine($"   ? Zero allocations - direct memory modification");

        // 2. Performance comparison
        Console.WriteLine("\n2. Performance Comparison: AsSpan vs Indexer\n");
        
        var largeList = new List<int>(100000);
        for (int i = 0; i < 100000; i++)
        {
            largeList.Add(i);
        }

        // Using indexer
        var sw = System.Diagnostics.Stopwatch.StartNew();
        long sum1 = 0;
        for (int i = 0; i < largeList.Count; i++)
        {
            sum1 += largeList[i];
        }
        sw.Stop();
        var indexerTime = sw.Elapsed.TotalMilliseconds;

        // Using AsSpan
        sw.Restart();
        long sum2 = 0;
        var listSpan = CollectionsMarshal.AsSpan(largeList);
        for (int i = 0; i < listSpan.Length; i++)
        {
            sum2 += listSpan[i];
        }
        sw.Stop();
        var spanTime = sw.Elapsed.TotalMilliseconds;

        Console.WriteLine($"   List size: {largeList.Count:N0} elements");
        Console.WriteLine($"   Indexer access: {indexerTime:F3}ms");
        Console.WriteLine($"   AsSpan access: {spanTime:F3}ms");
        Console.WriteLine($"   Improvement: {((indexerTime - spanTime) / indexerTime * 100):F1}% faster");

        // 3. GetValueRefOrNullRef for Dictionary<TKey, TValue>
        Console.WriteLine("\n3. GetValueRefOrNullRef() for Dictionary\n");
        
        var scores = new Dictionary<string, int>
        {
            ["Alice"] = 100,
            ["Bob"] = 85,
            ["Charlie"] = 92
        };

        Console.WriteLine("   Original scores:");
        foreach (var (name, score) in scores)
        {
            Console.WriteLine($"   - {name}: {score}");
        }

        // Traditional way - requires multiple lookups
        Console.WriteLine("\n   Traditional increment (2 dictionary lookups):");
        if (scores.ContainsKey("Alice"))
        {
            scores["Alice"] = scores["Alice"] + 10;
        }

        // CollectionsMarshal way - single lookup, direct reference
        Console.WriteLine("   CollectionsMarshal increment (1 lookup, direct ref):");
        ref var bobScore = ref CollectionsMarshal.GetValueRefOrNullRef(scores, "Bob");
        if (!Unsafe.IsNullRef(ref bobScore))
        {
            bobScore += 10; // Direct modification
        }

        Console.WriteLine("\n   Updated scores:");
        foreach (var (name, score) in scores)
        {
            Console.WriteLine($"   - {name}: {score}");
        }

        // 4. GetValueRefOrAddDefault - Get or add efficiently
        Console.WriteLine("\n4. GetValueRefOrAddDefault() - Efficient Get or Add\n");
        
        var inventory = new Dictionary<string, int>
        {
            ["Laptop"] = 10,
            ["Mouse"] = 50
        };

        Console.WriteLine("   Initial inventory:");
        foreach (var (item, count) in inventory)
        {
            Console.WriteLine($"   - {item}: {count}");
        }

        // Add or update efficiently
        ref var laptopCount = ref CollectionsMarshal.GetValueRefOrAddDefault(inventory, "Laptop", out bool existed);
        Console.WriteLine($"\n   'Laptop' existed: {existed}");
        laptopCount += 5; // Add 5 more laptops

        ref var keyboardCount = ref CollectionsMarshal.GetValueRefOrAddDefault(inventory, "Keyboard", out existed);
        Console.WriteLine($"   'Keyboard' existed: {existed}");
        keyboardCount = 20; // Set initial count

        Console.WriteLine("\n   Updated inventory:");
        foreach (var (item, count) in inventory)
        {
            Console.WriteLine($"   - {item}: {count}");
        }

        // 5. SetCount - Resize without initialization
        Console.WriteLine("\n5. SetCount() - Resize List Without Initialization\n");
        
        var buffer = new List<byte>();
        Console.WriteLine($"   Initial capacity: {buffer.Capacity}, Count: {buffer.Count}");

        // Traditional way - initializes all elements
        // buffer.AddRange(new byte[1000]); 

        // CollectionsMarshal way - just sets count, no initialization
        CollectionsMarshal.SetCount(buffer, 1000);
        Console.WriteLine($"   After SetCount(1000): Capacity: {buffer.Capacity}, Count: {buffer.Count}");
        Console.WriteLine($"   ? No initialization overhead - perfect for buffers");

        // Fill the buffer manually
        var bufferSpan = CollectionsMarshal.AsSpan(buffer);
        for (int i = 0; i < bufferSpan.Length; i++)
        {
            bufferSpan[i] = (byte)(i % 256);
        }

        Console.WriteLine($"   Buffer filled: first 10 bytes: {string.Join(", ", buffer.Take(10))}");

        // 6. Real-world example: High-performance data processing
        Console.WriteLine("\n6. Real-World Example: Batch Processing\n");
        
        var dataProcessor = new DataProcessor();
        var inputData = Enumerable.Range(1, 1000).ToList();
        
        var (traditionalTime, spanTime2) = dataProcessor.ProcessBatch(inputData);
        
        Console.WriteLine($"   Processed {inputData.Count} items:");
        Console.WriteLine($"   Traditional approach: {traditionalTime:F2}ms");
        Console.WriteLine($"   CollectionsMarshal approach: {spanTime2:F2}ms");
        Console.WriteLine($"   Improvement: {((traditionalTime - spanTime2) / traditionalTime * 100):F1}% faster");

        // 7. Dictionary counter optimization
        Console.WriteLine("\n7. Word Frequency Counter (Optimized)\n");
        
        var text = "the quick brown fox jumps over the lazy dog the fox was quick";
        var words = text.Split(' ');
        
        var wordCount = new Dictionary<string, int>();

        // Optimized counting using GetValueRefOrAddDefault
        foreach (var word in words)
        {
            ref var count = ref CollectionsMarshal.GetValueRefOrAddDefault(wordCount, word, out _);
            count++;
        }

        Console.WriteLine("   Word frequencies:");
        foreach (var (word, count) in wordCount.OrderByDescending(x => x.Value))
        {
            Console.WriteLine($"   - '{word}': {count}");
        }

        // 8. Important warnings
        Console.WriteLine("\n??  Important Warnings:\n");
        Console.WriteLine("   1. AsSpan() is invalidated if List is modified (Add, Remove, etc.)");
        Console.WriteLine("   2. SetCount() doesn't initialize - elements contain garbage");
        Console.WriteLine("   3. GetValueRefOrNullRef returns ref that can become invalid");
        Console.WriteLine("   4. These are low-level, unsafe operations");
        Console.WriteLine("   5. Use only in performance-critical paths");
        Console.WriteLine("   6. Always measure - micro-optimizations can hurt readability");

        // Key Benefits
        Console.WriteLine("\n?? CollectionsMarshal Benefits:");
        Console.WriteLine("   ? Zero-allocation collection access");
        Console.WriteLine("   ? Direct memory manipulation");
        Console.WriteLine("   ? Faster than indexer/lookup");
        Console.WriteLine("   ? Efficient get-or-add pattern");
        Console.WriteLine("   ? Buffer management without initialization");
        Console.WriteLine("   ? Perfect for hot paths in performance-critical code");
        Console.WriteLine("   ??  Advanced feature - use with care");

        Console.WriteLine();
    }

    private class DataProcessor
    {
        public (double traditionalTime, double spanTime) ProcessBatch(List<int> data)
        {
            // Traditional approach
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var results1 = new List<int>(data.Count);
            foreach (var item in data)
            {
                results1.Add(item * 2);
            }
            sw.Stop();
            var traditionalTime = sw.Elapsed.TotalMilliseconds;

            // CollectionsMarshal approach
            sw.Restart();
            var results2 = new List<int>();
            CollectionsMarshal.SetCount(results2, data.Count);
            var inputSpan = CollectionsMarshal.AsSpan(data);
            var outputSpan = CollectionsMarshal.AsSpan(results2);
            
            for (int i = 0; i < inputSpan.Length; i++)
            {
                outputSpan[i] = inputSpan[i] * 2;
            }
            sw.Stop();
            var spanTime = sw.Elapsed.TotalMilliseconds;

            return (traditionalTime, spanTime);
        }
    }
}
