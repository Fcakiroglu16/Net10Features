# .NET 10 & C# 14 Features Demonstration

A comprehensive showcase of the latest features introduced in .NET 10 and C# 14, with detailed examples and explanations.

## ?? Overview

This project demonstrates all major features introduced in .NET 10 and C# 14, organized into separate classes for easy learning and reference. Each feature includes practical examples with detailed comments explaining the concepts.

## ?? Requirements

- **.NET 10 SDK** or later
- **C# 14.0** language version
- Visual Studio 2025+ or any IDE supporting .NET 10

## ?? Features Covered

### 1. Collection Expression Improvements
**File:** `Features/CollectionExpressionImprovements.cs`

- Basic collection expressions syntax
- Spread operator (`..`) enhancements
- Nested collection expressions
- Support for different collection types (Span, HashSet, List, Array)
- Conditional spreading
- LINQ integration with collection expressions

```csharp
int[] numbers = [1, 2, 3, 4, 5];
int[] combined = [..numbers, 6, 7, 8];
HashSet<string> names = ["Alice", "Bob", "Charlie"];
```

### 2. Params Collection Enhancements
**File:** `Features/ParamsCollectionEnhancements.cs`

- `params` with `Span<T>` - stack-allocated efficiency
- `params` with `ReadOnlySpan<T>` - zero-copy performance
- `params` with `List<T>` and `IEnumerable<T>`
- Performance comparisons between different approaches

```csharp
void Process(params ReadOnlySpan<int> values) { }
Process(1, 2, 3, 4, 5); // No heap allocation!
```

### 3. LINQ Improvements
**File:** `Features/LinqImprovements.cs`

- **CountBy**: Count elements by key selector
- **AggregateBy**: Aggregate values by key
- **Index**: Get index alongside elements
- Complex aggregation examples

```csharp
var countByCategory = products.CountBy(p => p.Category);
var totalByCategory = products.AggregateBy(
    p => p.Category, 
    seed: 0m, 
    (total, p) => total + p.Price
);
```

### 4. Async/Await Improvements
**File:** `Features/AsyncImprovements.cs`

- **Task.WhenEach**: Process tasks as they complete
- Improved async state machine (performance optimizations)
- **ConfigureAwaitOptions**: More control over continuations
- Enhanced cancellation token support
- Better IAsyncEnumerable integration

```csharp
await foreach (var task in Task.WhenEach(tasks))
{
    var result = await task;
    // Process as soon as each task completes
}
```

### 5. Overload Resolution Improvements
**File:** `Features/OverloadResolutionImprovements.cs`

- Better type inference with collection expressions
- Improved generic method selection
- Enhanced lambda type inference
- Smarter params collection resolution
- Better nullable type handling

```csharp
// Compiler automatically chooses the best overload
ProcessData([1, 2, 3]); // Selects most efficient version
```

### 6. Primary Constructors in Structs
**File:** `Features/PrimaryConstructorsInStructs.cs`

- Basic struct primary constructors
- Readonly structs with primary constructors
- Generic structs
- Record structs with primary constructors
- Validation and computed properties

```csharp
public struct Point2D(double x, double y)
{
    public double X { get; } = x;
    public double Y { get; } = y;
    public double Distance() => Math.Sqrt(x * x + y * y);
}
```

### 7. Field Keyword in Properties
**File:** `Features/FieldKeywordInProperties.cs`

- Direct backing field access with `field` keyword
- Property validation using field
- Lazy initialization patterns
- Change notification implementation
- Caching strategies
- Init-only properties

```csharp
public string Name
{
    get => field;
    set => field = value?.Trim() ?? throw new ArgumentNullException();
}
```

### 8. Ref Struct Enhancements
**File:** `Features/RefStructEnhancements.cs`

- Ref structs with ref fields
- **allows ref struct** in generics (NEW!)
- Scoped parameters and lifetimes
- Ref returns in ref structs
- High-performance scenarios (parsing, buffering)

```csharp
public ref struct ByteWriter(Span<byte> buffer)
{
    public void WriteInt32(int value) { /* ... */ }
    public void WriteString(ReadOnlySpan<char> value) { /* ... */ }
}
```

## ?? Running the Project

### Option 1: Visual Studio
1. Open `Net10Features.sln`
2. Press `F5` or click "Run"

### Option 2: Command Line
```bash
cd Net10Features
dotnet run
```

### Option 3: Run Specific Features
Modify `Program.cs` to run only specific demonstrations:

```csharp
// Comment out features you don't want to see
CollectionExpressionImprovements.Demonstrate();
// ParamsCollectionEnhancements.Demonstrate();
// LinqImprovements.Demonstrate();
```

## ?? Project Structure

```
Net10Features/
??? Program.cs                          # Main entry point
??? Features/
?   ??? CollectionExpressionImprovements.cs
?   ??? ParamsCollectionEnhancements.cs
?   ??? LinqImprovements.cs
?   ??? AsyncImprovements.cs
?   ??? OverloadResolutionImprovements.cs
?   ??? PrimaryConstructorsInStructs.cs
?   ??? FieldKeywordInProperties.cs
?   ??? RefStructEnhancements.cs
??? README.md
```

## ?? Key Benefits

### Performance Improvements
- **Reduced allocations** with `params ReadOnlySpan<T>`
- **Stack-allocated collections** with ref structs
- **Optimized async state machines**
- **Zero-copy operations** with spans

### Developer Productivity
- **Less boilerplate** with collection expressions
- **Cleaner syntax** with primary constructors
- **Better type inference** reducing explicit type annotations
- **More expressive LINQ** with CountBy and AggregateBy

### Code Quality
- **Safer code** with field keyword validation
- **Better async patterns** with Task.WhenEach
- **Clearer intent** with explicit collection types
- **Stronger typing** with improved overload resolution

## ?? Learning Path

**Recommended order for learning:**

1. **Collection Expression Improvements** - Foundation for modern C# syntax
2. **Primary Constructors in Structs** - Understand the new struct syntax
3. **Field Keyword in Properties** - Master property patterns
4. **LINQ Improvements** - Essential for data processing
5. **Params Collection Enhancements** - Performance optimization
6. **Async/Await Improvements** - Modern async patterns
7. **Overload Resolution Improvements** - Advanced type system
8. **Ref Struct Enhancements** - High-performance scenarios

## ?? Code Examples

### Quick Start Examples

**Collection Expressions:**
```csharp
int[] numbers = [1, 2, 3];
List<string> names = ["Alice", "Bob"];
Span<int> span = [10, 20, 30];
```

**LINQ CountBy:**
```csharp
var wordCounts = words.CountBy(w => w.Length);
foreach (var (length, count) in wordCounts)
    Console.WriteLine($"Length {length}: {count} words");
```

**Task.WhenEach:**
```csharp
var tasks = urls.Select(url => FetchAsync(url));
await foreach (var completed in Task.WhenEach(tasks))
{
    var result = await completed;
    ProcessResult(result);
}
```

**Field Keyword:**
```csharp
public int Age
{
    get => field;
    set => field = value >= 0 ? value : throw new ArgumentException();
}
```

## ?? Additional Resources

- [What's New in .NET 10](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10/overview)
- [C# 14 Language Features](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-14)
- [Collection Expressions](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/collection-expressions)
- [Params Collections](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/params)

## ?? Contributing

This is a demonstration project. Feel free to:
- Add more examples
- Improve existing demonstrations
- Add performance benchmarks
- Enhance documentation

## ?? License

This project is provided as-is for educational purposes.

## ?? Notes

- All features require .NET 10 SDK and C# 14
- Some features may have preview status
- Performance characteristics may vary based on runtime version
- Examples are simplified for clarity

## ?? Updates

This project will be updated as new features are added to .NET 10 and C# 14.

---

**Happy Coding! ??**

For questions or suggestions, please refer to the official .NET documentation or community resources.
