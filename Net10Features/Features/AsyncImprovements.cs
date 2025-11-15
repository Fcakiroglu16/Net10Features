namespace Net10Features.Features;

/// <summary>
/// Demonstrates async/await improvements in .NET 10
/// Including Task.WhenEach, async method builder improvements, and better cancellation
/// </summary>
public class AsyncImprovements
{
    public static async Task DemonstrateAsync()
    {
        Console.WriteLine("=== Async/Await Improvements ===\n");

        // 1. Task.WhenEach - NEW in .NET 10
        // Process tasks as they complete, not waiting for all
        Console.WriteLine("1. Task.WhenEach - Process tasks as they complete:");
        await DemonstrateWhenEach();

        // 2. Improved async state machine
        Console.WriteLine("\n2. Improved async state machine (performance optimization):");
        await DemonstrateAsyncStateMachine();

        // 3. ConfigureAwait enhancements
        Console.WriteLine("\n3. ConfigureAwait with ConfigureAwaitOptions:");
        await DemonstrateConfigureAwaitOptions();

        // 4. Better cancellation token support
        Console.WriteLine("\n4. Enhanced CancellationToken support:");
        await DemonstrateCancellationImprovements();

        // 5. Async LINQ operations
        Console.WriteLine("\n5. Async LINQ with IAsyncEnumerable:");
        await DemonstrateAsyncLinq();

        Console.WriteLine();
    }

    // Task.WhenEach allows processing tasks as they complete
    private static async Task DemonstrateWhenEach()
    {
        var tasks = new[]
        {
            DelayedTaskAsync("Task 1", 300),
            DelayedTaskAsync("Task 2", 100),
            DelayedTaskAsync("Task 3", 200),
            DelayedTaskAsync("Task 4", 50)
        };

        // Process each task as it completes (not in original order)
        await foreach (var completedTask in Task.WhenEach(tasks))
        {
            var result = await completedTask;
            Console.WriteLine($"   Completed: {result}");
        }
    }

    private static async Task<string> DelayedTaskAsync(string name, int delay)
    {
        await Task.Delay(delay);
        return $"{name} (after {delay}ms)";
    }

    // Improved async state machine reduces allocations
    private static async Task DemonstrateAsyncStateMachine()
    {
        // In .NET 10, simple async methods have better performance
        // State machine optimizations reduce heap allocations
        var result = await FastAsyncMethodAsync();
        Console.WriteLine($"   Result: {result}");
        
        // Multiple awaits are also optimized
        var sum = await CalculateSumAsync(10, 20);
        Console.WriteLine($"   Sum: {sum}");
    }

    private static async Task<int> FastAsyncMethodAsync()
    {
        // Optimized state machine for simple cases
        await Task.Delay(10);
        return 42;
    }

    private static async Task<int> CalculateSumAsync(int a, int b)
    {
        await Task.Delay(10);
        var result = a + b;
        await Task.Delay(10);
        return result;
    }

    // ConfigureAwait with options
    private static async Task DemonstrateConfigureAwaitOptions()
    {
        // ConfigureAwaitOptions provides more control over continuation behavior
        var result1 = await GetDataAsync()
            .ConfigureAwait(ConfigureAwaitOptions.None);
        Console.WriteLine($"   Data with None option: {result1}");

        // Force continuation on thread pool
        var result2 = await GetDataAsync()
            .ConfigureAwait(ConfigureAwaitOptions.ForceYielding);
        Console.WriteLine($"   Data with ForceYielding: {result2}");
    }

    private static async Task<string> GetDataAsync()
    {
        await Task.Delay(10);
        return "Sample data";
    }

    // Enhanced cancellation token support
    private static async Task DemonstrateCancellationImprovements()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        
        try
        {
            // Better cancellation token flow
            await ProcessWithCancellationAsync(cts.Token);
            Console.WriteLine("   Processing completed successfully");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("   Operation was cancelled");
        }
    }

    private static async Task ProcessWithCancellationAsync(CancellationToken cancellationToken)
    {
        // Improved CancellationToken.Register performance
        await using var registration = cancellationToken.Register(() =>
        {
            Console.WriteLine("   Cancellation requested");
        });

        for (int i = 0; i < 3; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(100, cancellationToken);
            Console.WriteLine($"   Step {i + 1} completed");
        }
    }

    // IAsyncEnumerable improvements
    private static async Task DemonstrateAsyncLinq()
    {
        // Better support for async LINQ operations
        await foreach (var number in GenerateNumbersAsync().Where(n => n % 2 == 0))
        {
            Console.WriteLine($"   Even number: {number}");
        }
    }

    private static async IAsyncEnumerable<int> GenerateNumbersAsync()
    {
        for (int i = 1; i <= 10; i++)
        {
            await Task.Delay(10); // Simulate async work
            yield return i;
        }
    }

    // Parallel async operations with WhenEach
    public static async Task DemonstrateParallelProcessing()
    {
        Console.WriteLine("\n=== Parallel Processing with WhenEach ===");

        var urls = new[] { "url1", "url2", "url3", "url4", "url5" };
        var tasks = urls.Select(url => FetchDataAsync(url)).ToArray();

        var completed = 0;
        await foreach (var task in Task.WhenEach(tasks))
        {
            var result = await task;
            completed++;
            Console.WriteLine($"[{completed}/{tasks.Length}] {result}");
        }
    }

    private static async Task<string> FetchDataAsync(string url)
    {
        var delay = Random.Shared.Next(50, 200);
        await Task.Delay(delay);
        return $"Data from {url} (took {delay}ms)";
    }

    // Example: Timeout with modern async patterns
    public static async Task<T?> WithTimeoutAsync<T>(Task<T> task, TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);
        try
        {
            return await task.WaitAsync(cts.Token);
        }
        catch (TimeoutException)
        {
            Console.WriteLine("Operation timed out");
            return default;
        }
    }
}
