namespace Net10Features.Features;

/// <summary>
/// Demonstrates the 'field' keyword in properties - C# 14 feature
/// The 'field' keyword provides direct access to the backing field in property accessors
/// </summary>
public class FieldKeywordInProperties
{
    public static void Demonstrate()
    {
        Console.WriteLine("=== Field Keyword in Properties ===\n");

        // 1. Basic usage of field keyword
        Console.WriteLine("1. Basic field keyword usage:");
        var person = new Person { Name = "Alice", Age = 25 };
        Console.WriteLine($"   Person: {person.Name}, Age: {person.Age}");
        person.Age = 26;
        Console.WriteLine($"   Updated age: {person.Age}");

        // 2. Validation using field keyword
        Console.WriteLine("\n2. Validation with field keyword:");
        var product = new Product { Name = "Laptop", Price = 999.99m };
        Console.WriteLine($"   Product: {product.Name}, Price: ${product.Price}");
        try
        {
            product.Price = -50;
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"   Validation error: {ex.Message}");
        }

        // 3. Lazy initialization with field keyword
        Console.WriteLine("\n3. Lazy initialization:");
        var dataProcessor = new DataProcessor();
        Console.WriteLine($"   First access: {dataProcessor.ExpensiveData}");
        Console.WriteLine($"   Second access: {dataProcessor.ExpensiveData}");

        // 4. Change notification with field keyword
        Console.WriteLine("\n4. Change notification:");
        var observable = new ObservableValue { Value = 10 };
        observable.ValueChanged += (oldVal, newVal) =>
            Console.WriteLine($"   Value changed from {oldVal} to {newVal}");
        observable.Value = 20;
        observable.Value = 30;

        // 5. Caching with field keyword
        Console.WriteLine("\n5. Caching computed values:");
        var calculator = new Calculator();
        var result1 = calculator.ComplexCalculation;
        Console.WriteLine($"   First call: {result1}");
        var result2 = calculator.ComplexCalculation;
        Console.WriteLine($"   Second call (cached): {result2}");

        // 6. Field keyword with init-only properties
        Console.WriteLine("\n6. Init-only properties with field keyword:");
        var config = new AppConfiguration { MaxConnections = 100, Timeout = 30 };
        Console.WriteLine($"   Config: MaxConnections={config.MaxConnections}, Timeout={config.Timeout}s");

        Console.WriteLine();
    }
}

// 1. Basic field keyword usage
public class Person
{
    // Using 'field' keyword to access the backing field directly
    public string Name
    {
        get => field;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name cannot be empty");
            field = value.Trim();
        }
    }

    public int Age
    {
        get => field;
        set
        {
            if (value < 0 || value > 150)
                throw new ArgumentOutOfRangeException(nameof(value), "Age must be between 0 and 150");
            field = value;
        }
    }
}

// 2. Validation with field keyword
public class Product
{
    public string Name
    {
        get => field;
        set => field = value?.Trim() ?? throw new ArgumentNullException(nameof(value));
    }

    public decimal Price
    {
        get => field;
        set
        {
            if (value < 0)
                throw new ArgumentException("Price cannot be negative");
            
            // Log price changes
            if (field != value)
            {
                Console.WriteLine($"   Price changing from ${field:F2} to ${value:F2}");
            }
            
            field = value;
        }
    }

    public int Stock
    {
        get => field;
        set
        {
            field = Math.Max(0, value); // Ensure stock is never negative
            
            if (field == 0)
            {
                Console.WriteLine($"   Warning: {Name} is out of stock!");
            }
        }
    }
}

// 3. Lazy initialization using field keyword
public class DataProcessor
{
    // Lazy initialization - only computed on first access
    public string ExpensiveData
    {
        get
        {
            if (field == null)
            {
                Console.WriteLine("   Computing expensive data...");
                field = PerformExpensiveOperation();
            }
            return field;
        }
    }

    private string PerformExpensiveOperation()
    {
        // Simulate expensive operation
        System.Threading.Thread.Sleep(100);
        return "Expensive computed data";
    }
}

// 4. Change notification pattern with field keyword
public class ObservableValue
{
    public event Action<int, int>? ValueChanged;

    public int Value
    {
        get => field;
        set
        {
            if (field != value)
            {
                var oldValue = field;
                field = value;
                ValueChanged?.Invoke(oldValue, value);
            }
        }
    }
}

// 5. Caching with field keyword
public class Calculator
{
    private bool _isCalculated;

    public double ComplexCalculation
    {
        get
        {
            if (!_isCalculated)
            {
                Console.WriteLine("   Performing complex calculation...");
                field = Math.PI * Math.E * 42; // Simulate complex calculation
                _isCalculated = true;
            }
            return field;
        }
    }

    // Reset cache
    public void ResetCache()
    {
        _isCalculated = false;
    }
}

// 6. Init-only properties with field keyword
public class AppConfiguration
{
    public int MaxConnections
    {
        get => field;
        init
        {
            if (value <= 0)
                throw new ArgumentException("MaxConnections must be positive");
            field = value;
        }
    }

    public int Timeout
    {
        get => field;
        init
        {
            if (value < 0)
                throw new ArgumentException("Timeout cannot be negative");
            field = value;
        }
    }
}

// Example: Advanced usage with transformation
public class TemperatureSensor
{
    // Store in Celsius internally, but allow setting in any unit
    public double Celsius
    {
        get => field;
        set => field = value;
    }

    public double Fahrenheit
    {
        get => Celsius * 9 / 5 + 32;
        set => Celsius = (value - 32) * 5 / 9;
    }

    public double Kelvin
    {
        get => Celsius + 273.15;
        set => Celsius = value - 273.15;
    }
}

// Example: Field keyword with nullable reference types
public class UserProfile
{
    private DateTime _lastModified = DateTime.Now;

    public string Username
    {
        get => field;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Username cannot be empty");
            
            field = value;
            _lastModified = DateTime.Now;
        }
    }

    public string? Bio
    {
        get => field;
        set
        {
            // Allow null, but trim if not null
            field = value?.Trim();
            _lastModified = DateTime.Now;
        }
    }

    public DateTime LastModified => _lastModified;
}

// Example: Field keyword with collections
public class TodoList
{
    public List<string> Items
    {
        get => field ??= new List<string>(); // Lazy initialization
        set => field = value ?? throw new ArgumentNullException(nameof(value));
    }

    public int Count => Items.Count;

    public void AddItem(string item)
    {
        if (!string.IsNullOrWhiteSpace(item))
        {
            Items.Add(item);
            Console.WriteLine($"   Added todo: {item} (Total: {Count})");
        }
    }
}

// Example: Coercion with field keyword
public class Volume
{
    public double Liters
    {
        get => field;
        set
        {
            // Coerce value to valid range
            field = Math.Clamp(value, 0, 1000);
            
            if (value != field)
            {
                Console.WriteLine($"   Value {value} coerced to {field}");
            }
        }
    }

    public double Milliliters
    {
        get => Liters * 1000;
        set => Liters = value / 1000;
    }
}
