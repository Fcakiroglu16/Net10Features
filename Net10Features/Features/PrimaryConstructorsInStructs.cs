namespace Net10Features.Features;

/// <summary>
/// Demonstrates primary constructors in structs - C# 14 feature
/// Primary constructors were added for classes in C# 12, now extended to structs
/// </summary>
public class PrimaryConstructorsInStructs
{
    public static void Demonstrate()
    {
        Console.WriteLine("=== Primary Constructors in Structs ===\n");

        // 1. Basic struct with primary constructor
        Console.WriteLine("1. Basic struct with primary constructor:");
        var point = new Point2D(10, 20);
        Console.WriteLine($"   Point: ({point.X}, {point.Y})");
        Console.WriteLine($"   Distance from origin: {point.DistanceFromOrigin():F2}");

        // 2. Struct with multiple parameters
        Console.WriteLine("\n2. Struct with validation in primary constructor:");
        try
        {
            var temp = new Temperature(25.5, TemperatureUnit.Celsius);
            Console.WriteLine($"   Temperature: {temp}");
            Console.WriteLine($"   In Fahrenheit: {temp.ToFahrenheit():F2}°F");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"   Error: {ex.Message}");
        }

        // 3. Readonly struct with primary constructor
        Console.WriteLine("\n3. Readonly struct with primary constructor:");
        var vector = new Vector3D(1.0, 2.0, 3.0);
        Console.WriteLine($"   Vector: {vector}");
        Console.WriteLine($"   Magnitude: {vector.Magnitude:F2}");

        // 4. Struct with computed properties
        Console.WriteLine("\n4. Struct with computed properties:");
        var rect = new Rectangle(5, 10);
        Console.WriteLine($"   Rectangle: {rect.Width} x {rect.Height}");
        Console.WriteLine($"   Area: {rect.Area}");
        Console.WriteLine($"   Perimeter: {rect.Perimeter}");

        // 5. Struct implementing interfaces
        Console.WriteLine("\n5. Struct implementing interfaces:");
        var person = new PersonStruct("John Doe", 30);
        Console.WriteLine($"   {person.GetDescription()}");
        Console.WriteLine($"   Hash: {person.GetHashCode()}");

        // 6. Generic struct with primary constructor
        Console.WriteLine("\n6. Generic struct with primary constructor:");
        var result = new Result<int>(true, 42, null);
        Console.WriteLine($"   Success: {result.IsSuccess}, Value: {result.Value}");
        
        var errorResult = new Result<string>(false, null, "Operation failed");
        Console.WriteLine($"   Success: {errorResult.IsSuccess}, Error: {errorResult.Error}");

        // 7. Struct with default values and optional parameters
        Console.WriteLine("\n7. Combining primary constructors with methods:");
        var config = new Configuration("MyApp", "1.0.0");
        Console.WriteLine($"   {config}");
        var updated = config.WithPort(8080);
        Console.WriteLine($"   Updated: {updated}");

        Console.WriteLine();
    }
}

// 1. Simple struct with primary constructor
public struct Point2D(double x, double y)
{
    // Primary constructor parameters are automatically available as private fields
    public double X { get; } = x;
    public double Y { get; } = y;

    public double DistanceFromOrigin() => Math.Sqrt(x * x + y * y);

    public override string ToString() => $"({X}, {Y})";
}

// 2. Struct with validation
public struct Temperature(double value, TemperatureUnit unit)
{
    // Validation in property initializers
    public double Value { get; } = value;
    public TemperatureUnit Unit { get; } = unit;

    // Use primary constructor parameters in methods
    public double ToFahrenheit() => unit switch
    {
        TemperatureUnit.Celsius => value * 9 / 5 + 32,
        TemperatureUnit.Fahrenheit => value,
        TemperatureUnit.Kelvin => (value - 273.15) * 9 / 5 + 32,
        _ => throw new ArgumentException("Invalid unit")
    };

    public override string ToString() => $"{Value}° {Unit}";
}

public enum TemperatureUnit
{
    Celsius,
    Fahrenheit,
    Kelvin
}

// 3. Readonly struct with primary constructor
public readonly struct Vector3D(double x, double y, double z)
{
    // All properties are readonly
    public double X { get; } = x;
    public double Y { get; } = y;
    public double Z { get; } = z;

    public double Magnitude => Math.Sqrt(x * x + y * y + z * z);

    public Vector3D Normalize()
    {
        var mag = Magnitude;
        return new Vector3D(x / mag, y / mag, z / mag);
    }

    public override string ToString() => $"({X}, {Y}, {Z})";
}

// 4. Struct with computed properties using primary constructor parameters
public struct Rectangle(double width, double height)
{
    public double Width { get; } = width > 0 ? width : throw new ArgumentException("Width must be positive");
    public double Height { get; } = height > 0 ? height : throw new ArgumentException("Height must be positive");

    // Computed properties can use primary constructor parameters
    public double Area => width * height;
    public double Perimeter => 2 * (width + height);
    public double Diagonal => Math.Sqrt(width * width + height * height);

    public bool IsSquare => Math.Abs(width - height) < 0.0001;
}

// 5. Struct implementing interface with primary constructor
public struct PersonStruct(string name, int age) : IDescribable
{
    public string Name { get; } = name;
    public int Age { get; } = age;

    public string GetDescription() => $"{Name}, {Age} years old";

    public override int GetHashCode() => HashCode.Combine(Name, Age);
    public override bool Equals(object? obj) =>
        obj is PersonStruct other && Name == other.Name && Age == other.Age;
}

public interface IDescribable
{
    string GetDescription();
}

// 6. Generic struct with primary constructor
public struct Result<T>(bool isSuccess, T? value, string? error)
{
    public bool IsSuccess { get; } = isSuccess;
    public T? Value { get; } = value;
    public string? Error { get; } = error;

    // Helper methods
    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<string, TResult> onFailure) =>
        isSuccess ? onSuccess(value!) : onFailure(error!);
}

// 7. Struct with with-expression support (record struct)
public record struct Configuration(string AppName, string Version)
{
    // Additional properties with default values
    public int Port { get; init; } = 80;
    public string Environment { get; init; } = "Development";

    // Methods using primary constructor parameters
    public Configuration WithPort(int newPort) => this with { Port = newPort };
    public Configuration WithEnvironment(string env) => this with { Environment = env };

    public override string ToString() =>
        $"{AppName} v{Version} on port {Port} ({Environment})";
}

// Example: Complex struct with multiple features
public readonly struct Money(decimal amount, string currency)
{
    public decimal Amount { get; } = amount;
    public string Currency { get; } = currency ?? throw new ArgumentNullException(nameof(currency));

    // Operators using primary constructor parameters
    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies");
        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator -(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Cannot subtract money with different currencies");
        return new Money(a.Amount - b.Amount, a.Currency);
    }

    public override string ToString() => $"{Amount:F2} {Currency}";
}
