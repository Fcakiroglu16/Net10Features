# .NET 9 Features Demonstration

This folder contains comprehensive examples of new features introduced in .NET 9.

## Features Demonstrated

### 1. **LINQ Improvements** (`LinqImprovements.cs`)
New LINQ methods for better performance and expressiveness.

**New Methods:**
- **CountBy()** - Count occurrences by key
- **AggregateBy()** - Aggregate values by key without GroupBy
- **Index()** - Enumerate with indices

**Example:**
```csharp
// CountBy - simpler and faster than GroupBy + Select
var orderCounts = orders.CountBy(o => o.CustomerName);

// AggregateBy - efficient aggregation
var totalsByCustomer = orders.AggregateBy(
    keySelector: o => o.CustomerName,
    seed: 0m,
    func: (total, order) => total + order.Amount);

// Index - cleaner enumeration with indices
foreach (var (index, product) in products.Index())
{
    Console.WriteLine($"[{index}] {product}");
}
```

**Benefits:**
- ? Better performance than GroupBy
- ? Less memory allocations
- ? More expressive code
- ? Simpler syntax

---

### 2. **TimeProvider** (`TimeProviderFeature.cs`)
Built-in time abstraction for testable time-dependent code.

**Key Features:**
- System time provider for production
- Fake time provider for testing
- Timer support
- Timezone-aware operations

**Example:**
```csharp
// Production code
public class OrderService
{
    private readonly TimeProvider _timeProvider;
    
    public OrderService(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }
    
    public Order CreateOrder()
    {
        return new Order 
        { 
            CreatedAt = _timeProvider.GetUtcNow() 
        };
    }
}

// Testing
var fakeTime = new FakeTimeProvider();
fakeTime.SetUtcNow(new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero));

var service = new OrderService(fakeTime);
var order = service.CreateOrder(); // Uses fake time

fakeTime.Advance(TimeSpan.FromHours(1)); // Control time in tests
```

**Benefits:**
- ? Built-in abstraction (no third-party libs)
- ? Easy testing
- ? Controllable time in tests
- ? Timer support

---

### 3. **SearchValues** (`SearchValuesOptimization.cs`)
SIMD-accelerated searching for sets of values.

**Key Features:**
- Optimized for repeated searches
- SIMD acceleration
- Works with chars, strings, bytes
- Case-insensitive string searching

**Example:**
```csharp
// Create once, use many times
var vowels = SearchValues.Create("aeiouAEIOU");

// Much faster than Contains/Any
var text = "Hello World";
var hasVowels = text.AsSpan().ContainsAny(vowels);
var vowelIndex = text.AsSpan().IndexOfAny(vowels);

// String searching
var badWords = SearchValues.Create(
    ["spam", "scam", "fraud"], 
    StringComparison.OrdinalIgnoreCase);
    
var containsBadWord = message.AsSpan().ContainsAny(badWords);
```

**Performance:**
- ? Up to 10x faster than Contains/Any
- ? SIMD-accelerated
- ? Zero allocations after creation

---

### 4. **CollectionsMarshal** (`CollectionsMarshalFeatures.cs`)
Low-level, high-performance collection access.

**Key Features:**
- Direct memory access to collections
- AsSpan() for List<T>
- SetCount() without initialization
- GetValueRefOrNullRef() for Dictionary

**Example:**
```csharp
var list = new List<int> { 1, 2, 3, 4, 5 };

// Get span over internal array
var span = CollectionsMarshal.AsSpan(list);
for (int i = 0; i < span.Length; i++)
{
    span[i] *= 2; // Direct modification
}

// Dictionary: single lookup, direct reference
var dict = new Dictionary<string, int>();
ref var value = ref CollectionsMarshal.GetValueRefOrAddDefault(
    dict, "key", out bool existed);
value = 100;

// SetCount without initialization
var buffer = new List<byte>();
CollectionsMarshal.SetCount(buffer, 1000); // No initialization
```

**Benefits:**
- ? Zero-allocation access
- ? Direct memory manipulation
- ? Faster than indexer/lookup
- ?? Advanced feature - use with care

---

### 5. **Tensor<T> and AI** (`TensorAndAIFeatures.cs`)
High-performance multi-dimensional arrays for AI/ML workloads.

**Key Features:**
- Multi-dimensional arrays
- SIMD-accelerated operations
- Built-in ML primitives
- Activation functions

**Example:**
```csharp
// Create tensors
var matrix = new DenseTensor<float>(new[] { 3, 3 });

// SIMD-accelerated operations
var a = new float[] { 1, 2, 3, 4 };
var b = new float[] { 5, 6, 7, 8 };
var result = new float[4];

TensorPrimitives.Add(a, b, result);
TensorPrimitives.Multiply(a, b, result);

// Statistical operations
var sum = TensorPrimitives.Sum(data);
var mean = sum / data.Length;
var max = TensorPrimitives.Max(data);

// ML operations
var distance = TensorPrimitives.Distance(point1, point2);
var similarity = TensorPrimitives.CosineSimilarity(vec1, vec2);

// Activation functions
TensorPrimitives.Sigmoid(inputs, outputs);
TensorPrimitives.Tanh(inputs, outputs);
```

**Performance:**
- ? 10-100x faster than regular loops
- ? SIMD acceleration
- ? Hardware acceleration support

---

### 6. **Regex Improvements** (`RegexImprovements.cs`)
Source-generated regex for better performance.

**Key Features:**
- Compile-time regex compilation
- Source generators
- Better performance
- Timeout support

**Example:**
```csharp
// Source-generated regex (compile-time)
[GeneratedRegex(@"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}\b", 
    RegexOptions.IgnoreCase)]
private static partial Regex EmailRegex();

// Use it
var isValid = EmailRegex().IsMatch("test@example.com");

// Named groups
var dateRegex = new Regex(@"(?<year>\d{4})-(?<month>\d{2})-(?<day>\d{2})");
var match = dateRegex.Match("2024-01-15");
var year = match.Groups["year"].Value;

// Timeout protection
var regex = new Regex(pattern, RegexOptions.None, TimeSpan.FromMilliseconds(100));
```

**Benefits:**
- ? Up to 2x faster
- ? Zero startup overhead
- ? Compile-time errors
- ? Reduced allocations

---

## Running the Demos

Each feature class has a `Demonstrate()` or `DemonstrateAsync()` method that shows:
1. Basic usage examples
2. Performance comparisons
3. Real-world scenarios
4. Best practices

## Key Takeaways

.NET 9 focuses on:
- ?? **Performance** - SIMD acceleration everywhere
- ? **Speed** - Faster LINQ, Regex, Collections
- ?? **Testability** - TimeProvider abstraction
- ?? **AI/ML** - Tensor<T> and primitives
- ?? **Low-level control** - CollectionsMarshal

## Performance Highlights

| Feature | Improvement |
|---------|-------------|
| LINQ CountBy | 30-50% faster than GroupBy |
| SearchValues | Up to 10x faster than Contains |
| CollectionsMarshal AsSpan | 20-40% faster than indexer |
| Tensor primitives | 10-100x faster than loops |
| Source-generated Regex | Up to 2x faster, zero startup |

## Comparison with Previous Versions

| Operation | Before .NET 9 | .NET 9 |
|-----------|--------------|--------|
| Count by key | GroupBy + Select | CountBy() |
| Time testing | Custom abstraction | TimeProvider |
| Multi-search | Multiple Contains | SearchValues |
| List to Span | ToArray() or custom | CollectionsMarshal.AsSpan() |
| ML arrays | Third-party | Tensor<T> |
| Regex compile | Runtime | Source-generated |

---

## Additional Resources

- [.NET 9 Release Notes](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9)
- [LINQ Improvements](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9#linq)
- [TimeProvider](https://learn.microsoft.com/en-us/dotnet/api/system.timeprovider)
- [SearchValues](https://learn.microsoft.com/en-us/dotnet/api/system.buffers.searchvalues)
- [Tensor<T>](https://learn.microsoft.com/en-us/dotnet/api/system.numerics.tensors.tensor-1)
