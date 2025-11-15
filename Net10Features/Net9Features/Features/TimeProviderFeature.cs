namespace Net10Features.Net9Features.Features;

/// <summary>
/// Demonstrates TimeProvider in .NET 9
/// TimeProvider is an abstraction over DateTime/DateTimeOffset that enables
/// testable time-dependent code. It's similar to ISystemClock but built into .NET.
/// </summary>
public class TimeProviderFeature
{
    public static async Task DemonstrateAsync()
    {
        Console.WriteLine("=== .NET 9: TimeProvider ===\n");

        // 1. System TimeProvider (real time)
        Console.WriteLine("1. System TimeProvider (Real Time)\n");
        
        var systemTime = TimeProvider.System;
        
        Console.WriteLine($"   Current UTC time: {systemTime.GetUtcNow():yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"   Current local time: {systemTime.GetLocalNow():yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"   Timezone: {systemTime.LocalTimeZone.DisplayName}");
        Console.WriteLine($"   Timestamp: {systemTime.GetTimestamp()}");

        // 2. The problem with DateTime.UtcNow
        Console.WriteLine("\n2. The Problem: Testing time-dependent code\n");
        
        Console.WriteLine("   ? Before .NET 9:");
        Console.WriteLine("   - DateTime.UtcNow is not mockable");
        Console.WriteLine("   - Hard to test time-dependent logic");
        Console.WriteLine("   - Need third-party libraries or custom abstractions\n");
        
        Console.WriteLine("   ? With .NET 9 TimeProvider:");
        Console.WriteLine("   - Built-in abstraction");
        Console.WriteLine("   - Easy to test with FakeTimeProvider");
        Console.WriteLine("   - Standard across .NET ecosystem");

        // 3. Using TimeProvider in services
        Console.WriteLine("\n3. Using TimeProvider in Services\n");
        
        var orderService = new OrderService(TimeProvider.System);
        var order = orderService.CreateOrder("Laptop", 999.99m);
        
        Console.WriteLine($"   Order created:");
        Console.WriteLine($"   - Product: {order.ProductName}");
        Console.WriteLine($"   - Price: ${order.Price}");
        Console.WriteLine($"   - Created: {order.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"   - Is Recent: {order.IsRecent(TimeProvider.System)}");

        // 4. Fake TimeProvider for testing
        Console.WriteLine("\n4. FakeTimeProvider for Testing\n");
        
        var fakeTime = new Microsoft.Extensions.Time.Testing.FakeTimeProvider();
        fakeTime.SetUtcNow(new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero));
        
        Console.WriteLine($"   Fake time set to: {fakeTime.GetUtcNow():yyyy-MM-dd HH:mm:ss}");
        
        var testOrderService = new OrderService(fakeTime);
        var testOrder = testOrderService.CreateOrder("Test Product", 100m);
        
        Console.WriteLine($"\n   Test order created at fake time:");
        Console.WriteLine($"   - Created: {testOrder.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        
        // Advance time
        fakeTime.Advance(TimeSpan.FromHours(1));
        Console.WriteLine($"\n   Time advanced by 1 hour: {fakeTime.GetUtcNow():yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"   - Is Recent: {testOrder.IsRecent(fakeTime)}");
        
        // Advance more
        fakeTime.Advance(TimeSpan.FromHours(25));
        Console.WriteLine($"\n   Time advanced by 25 hours: {fakeTime.GetUtcNow():yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"   - Is Recent: {testOrder.IsRecent(fakeTime)}");

        // 5. Timers with TimeProvider
        Console.WriteLine("\n5. Timers with TimeProvider\n");
        
        Console.WriteLine("   Creating timer with System TimeProvider:");
        var callCount = 0;
        using var timer = systemTime.CreateTimer(
            callback: _ => 
            {
                callCount++;
                Console.WriteLine($"   Timer tick #{callCount} at {systemTime.GetUtcNow():HH:mm:ss.fff}");
            },
            state: null,
            dueTime: TimeSpan.FromMilliseconds(100),
            period: TimeSpan.FromMilliseconds(200));

        await Task.Delay(600); // Wait for a few ticks
        Console.WriteLine($"   Timer ticked {callCount} times");

        // 6. Testing timers with FakeTimeProvider
        Console.WriteLine("\n6. Testing Timers with FakeTimeProvider\n");
        
        var fakeTimer = new Microsoft.Extensions.Time.Testing.FakeTimeProvider();
        var timerCallCount = 0;
        
        using var testTimer = fakeTimer.CreateTimer(
            callback: _ => 
            {
                timerCallCount++;
                Console.WriteLine($"   Fake timer tick #{timerCallCount}");
            },
            state: null,
            dueTime: TimeSpan.FromSeconds(1),
            period: TimeSpan.FromSeconds(1));

        Console.WriteLine("   Advancing fake time by 3.5 seconds:");
        fakeTimer.Advance(TimeSpan.FromSeconds(3.5));
        
        // Give a moment for callbacks to execute
        await Task.Delay(50);
        Console.WriteLine($"   Timer ticked {timerCallCount} times (should be 3)");

        // 7. Measuring elapsed time
        Console.WriteLine("\n7. Measuring Elapsed Time\n");
        
        var startTimestamp = systemTime.GetTimestamp();
        
        // Simulate some work
        await Task.Delay(100);
        
        var endTimestamp = systemTime.GetTimestamp();
        var elapsed = systemTime.GetElapsedTime(startTimestamp, endTimestamp);
        
        Console.WriteLine($"   Operation took: {elapsed.TotalMilliseconds:F2}ms");

        // 8. Timezone-aware operations
        Console.WriteLine("\n8. Timezone-Aware Operations\n");
        
        var utcNow = systemTime.GetUtcNow();
        var localNow = systemTime.GetLocalNow();
        var offset = localNow.Offset;
        
        Console.WriteLine($"   UTC: {utcNow:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"   Local: {localNow:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"   Offset: {offset.Hours} hours");

        // 9. Real-world example: Session management
        Console.WriteLine("\n9. Real-World Example: Session Management\n");
        
        var sessionManager = new SessionManager(TimeProvider.System);
        var sessionId = sessionManager.CreateSession("user123");
        
        Console.WriteLine($"   Session created: {sessionId}");
        Console.WriteLine($"   Is valid: {sessionManager.IsSessionValid(sessionId)}");
        
        // Test with fake time
        var testSessionManager = new SessionManager(fakeTime);
        fakeTime.SetUtcNow(DateTimeOffset.UtcNow);
        
        var testSessionId = testSessionManager.CreateSession("testuser");
        Console.WriteLine($"\n   Test session created with fake time");
        Console.WriteLine($"   Is valid: {testSessionManager.IsSessionValid(testSessionId)}");
        
        // Expire the session
        fakeTime.Advance(TimeSpan.FromMinutes(31));
        Console.WriteLine($"\n   Advanced time by 31 minutes");
        Console.WriteLine($"   Is valid: {testSessionManager.IsSessionValid(testSessionId)} (expired)");

        // Key Benefits
        Console.WriteLine("\n?? TimeProvider Benefits:");
        Console.WriteLine("   ? Built-in time abstraction");
        Console.WriteLine("   ? Testable time-dependent code");
        Console.WriteLine("   ? FakeTimeProvider for deterministic tests");
        Console.WriteLine("   ? Timer support with controllable time");
        Console.WriteLine("   ? Timestamp and elapsed time measurement");
        Console.WriteLine("   ? Timezone-aware operations");
        Console.WriteLine("   ? No third-party dependencies needed");

        Console.WriteLine();
    }

    // Example service using TimeProvider
    private class OrderService
    {
        private readonly TimeProvider _timeProvider;

        public OrderService(TimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
        }

        public Order CreateOrder(string productName, decimal price)
        {
            return new Order
            {
                Id = Guid.NewGuid(),
                ProductName = productName,
                Price = price,
                CreatedAt = _timeProvider.GetUtcNow()
            };
        }
    }

    private class Order
    {
        public Guid Id { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public DateTimeOffset CreatedAt { get; init; }

        public bool IsRecent(TimeProvider timeProvider)
        {
            var now = timeProvider.GetUtcNow();
            var age = now - CreatedAt;
            return age.TotalHours < 24;
        }
    }

    // Example: Session manager using TimeProvider
    private class SessionManager
    {
        private readonly TimeProvider _timeProvider;
        private readonly Dictionary<string, DateTimeOffset> _sessions = new();
        private readonly TimeSpan _sessionTimeout = TimeSpan.FromMinutes(30);

        public SessionManager(TimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
        }

        public string CreateSession(string userId)
        {
            var sessionId = Guid.NewGuid().ToString();
            _sessions[sessionId] = _timeProvider.GetUtcNow();
            return sessionId;
        }

        public bool IsSessionValid(string sessionId)
        {
            if (!_sessions.TryGetValue(sessionId, out var createdAt))
                return false;

            var now = _timeProvider.GetUtcNow();
            var age = now - createdAt;
            return age < _sessionTimeout;
        }
    }
}
