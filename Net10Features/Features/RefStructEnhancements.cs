namespace Net10Features.Features;

/// <summary>
/// Demonstrates ref struct enhancements in C# 14 / .NET 10
/// Including 'allows ref struct' feature for generic constraints
/// </summary>
public class RefStructEnhancements
{
    public static void Demonstrate()
    {
        Console.WriteLine("=== Ref Struct Enhancements ===\n");

        // 1. Basic ref struct usage
        Console.WriteLine("1. Basic ref struct:");
        Span<int> numbers = stackalloc int[] { 1, 2, 3, 4, 5 };
        var processor = new SpanProcessor(numbers);
        processor.Process();

        // 2. Ref struct with ref fields
        Console.WriteLine("\n2. Ref struct with ref fields:");
        int value = 100;
        var refHolder = new RefFieldHolder(ref value);
        Console.WriteLine($"   Original value: {value}");
        refHolder.Increment();
        Console.WriteLine($"   After increment: {value}");

        // 3. Allows ref struct in generics - NEW in C# 14
        Console.WriteLine("\n3. Allows ref struct in generics:");
        Span<int> data = stackalloc int[] { 5, 2, 8, 1, 9 };
        var max = GenericProcessor.FindMax(data);
        Console.WriteLine($"   Max value in span: {max}");

        // 4. Ref struct implementing interfaces
        Console.WriteLine("\n4. Ref struct with interface implementations:");
        Span<char> chars = stackalloc char[] { 'H', 'e', 'l', 'l', 'o' };
        var charSpan = new CharSpan(chars);
        Console.WriteLine($"   Length: {charSpan.Length}");
        Console.WriteLine($"   Content: {charSpan.GetContent()}");

        // 5. Scoped ref struct
        Console.WriteLine("\n5. Scoped ref parameters:");
        Span<int> buffer = stackalloc int[10];
        FillBuffer(buffer);
        Console.WriteLine($"   Buffer filled: {string.Join(", ", buffer.ToArray())}");

        // 6. Ref struct with methods returning ref
        Console.WriteLine("\n6. Ref returns in ref structs:");
        Span<int> values = stackalloc int[] { 10, 20, 30, 40, 50 };
        var accessor = new SpanAccessor(values);
        ref int element = ref accessor.GetReference(2);
        Console.WriteLine($"   Element at index 2: {element}");
        element = 300;
        Console.WriteLine($"   After modification: {values[2]}");

        // 7. Advanced ref struct patterns
        Console.WriteLine("\n7. Advanced ref struct patterns:");
        DemonstrateAdvancedPatterns();

        Console.WriteLine();
    }

    private static void FillBuffer(Span<int> buffer)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = i * 10;
        }
    }

    private static void DemonstrateAdvancedPatterns()
    {
        Span<byte> bytes = stackalloc byte[100];
        var writer = new ByteWriter(bytes);
        
        writer.WriteInt32(42);
        writer.WriteString("Hello");
        
        Console.WriteLine($"   Bytes written: {writer.Position}");
    }
}

// 1. Basic ref struct
public ref struct SpanProcessor
{
    private readonly Span<int> _data;

    public SpanProcessor(Span<int> data)
    {
        _data = data;
    }

    public void Process()
    {
        Console.WriteLine($"   Processing {_data.Length} items");
        for (int i = 0; i < _data.Length; i++)
        {
            _data[i] *= 2;
        }
        Console.WriteLine($"   Result: {string.Join(", ", _data.ToArray())}");
    }
}

// 2. Ref struct with ref fields - Enhanced in C# 14
public ref struct RefFieldHolder
{
    // Can hold a reference to a variable
    private ref int _value;

    public RefFieldHolder(ref int value)
    {
        _value = ref value;
    }

    public void Increment()
    {
        _value++;
    }

    public void SetValue(int newValue)
    {
        _value = newValue;
    }

    public int GetValue() => _value;
}

// 3. Generic class that can work with ref structs - NEW 'allows ref struct' feature
public static class GenericProcessor
{
    // The 'allows ref struct' constraint allows Span<T> and other ref structs
    public static T FindMax<T>(Span<T> items) where T : IComparable<T>
    {
        if (items.IsEmpty)
            throw new ArgumentException("Span cannot be empty");

        T max = items[0];
        foreach (var item in items)
        {
            if (item.CompareTo(max) > 0)
                max = item;
        }
        return max;
    }

    public static void ProcessItems<T>(Span<T> items, Action<T> action)
    {
        foreach (var item in items)
        {
            action(item);
        }
    }

    // Generic method that works with ref struct
    public static int Sum(ReadOnlySpan<int> values)
    {
        int sum = 0;
        foreach (var value in values)
            sum += value;
        return sum;
    }
}

// 4. Ref struct implementing interfaces (limited scenarios)
public interface IHasLength
{
    int Length { get; }
}

public ref struct CharSpan
{
    private readonly Span<char> _chars;

    public CharSpan(Span<char> chars)
    {
        _chars = chars;
    }

    public int Length => _chars.Length;

    public string GetContent() => new string(_chars);

    public void Clear()
    {
        _chars.Clear();
    }
}

// 5. Ref struct with scoped parameters
public ref struct ScopedBuffer
{
    private Span<byte> _buffer;

    public ScopedBuffer(Span<byte> buffer)
    {
        _buffer = buffer;
    }

    public void Write(scoped ReadOnlySpan<byte> data)
    {
        data.CopyTo(_buffer);
    }
}

// 6. Ref struct with ref returns
public ref struct SpanAccessor
{
    private Span<int> _data;

    public SpanAccessor(Span<int> data)
    {
        _data = data;
    }

    // Return a reference to an element
    public ref int GetReference(int index)
    {
        if (index < 0 || index >= _data.Length)
            throw new IndexOutOfRangeException();
        
        return ref _data[index];
    }

    // Get a slice
    public Span<int> GetSlice(int start, int length)
    {
        return _data.Slice(start, length);
    }
}

// 7. Advanced ref struct example - byte writer
public ref struct ByteWriter
{
    private Span<byte> _buffer;
    private int _position;

    public ByteWriter(Span<byte> buffer)
    {
        _buffer = buffer;
        _position = 0;
    }

    public int Position => _position;
    public int Remaining => _buffer.Length - _position;

    public void WriteInt32(int value)
    {
        if (Remaining < sizeof(int))
            throw new InvalidOperationException("Not enough space");

        BitConverter.TryWriteBytes(_buffer[_position..], value);
        _position += sizeof(int);
    }

    public void WriteString(ReadOnlySpan<char> value)
    {
        var byteCount = System.Text.Encoding.UTF8.GetByteCount(value);
        if (Remaining < byteCount)
            throw new InvalidOperationException("Not enough space");

        System.Text.Encoding.UTF8.GetBytes(value, _buffer[_position..]);
        _position += byteCount;
    }

    public void WriteByte(byte value)
    {
        if (Remaining < 1)
            throw new InvalidOperationException("Not enough space");

        _buffer[_position++] = value;
    }
}

// Example: Ref struct for parsing
public ref struct TokenParser
{
    private ReadOnlySpan<char> _input;
    private int _position;

    public TokenParser(ReadOnlySpan<char> input)
    {
        _input = input;
        _position = 0;
    }

    public ReadOnlySpan<char> ReadToken()
    {
        // Skip whitespace
        while (_position < _input.Length && char.IsWhiteSpace(_input[_position]))
            _position++;

        if (_position >= _input.Length)
            return ReadOnlySpan<char>.Empty;

        var start = _position;
        while (_position < _input.Length && !char.IsWhiteSpace(_input[_position]))
            _position++;

        return _input[start.._position];
    }

    public bool HasMore => _position < _input.Length;
}

// Example: Ref struct with enumerator
public ref struct SpanEnumerator<T>
{
    private readonly ReadOnlySpan<T> _span;
    private int _index;

    public SpanEnumerator(ReadOnlySpan<T> span)
    {
        _span = span;
        _index = -1;
    }

    public bool MoveNext()
    {
        _index++;
        return _index < _span.Length;
    }

    public readonly ref readonly T Current => ref _span[_index];
}

// Example: Stack-only data structure
public ref struct StackOnlyList<T>
{
    private Span<T> _items;
    private int _count;

    public StackOnlyList(Span<T> buffer)
    {
        _items = buffer;
        _count = 0;
    }

    public void Add(T item)
    {
        if (_count >= _items.Length)
            throw new InvalidOperationException("Buffer full");
        
        _items[_count++] = item;
    }

    public readonly int Count => _count;

    public readonly ReadOnlySpan<T> AsSpan() => _items[.._count];
}

// Example: Ref struct for high-performance string building
public ref struct StackStringBuilder
{
    private Span<char> _buffer;
    private int _position;

    public StackStringBuilder(Span<char> buffer)
    {
        _buffer = buffer;
        _position = 0;
    }

    public void Append(ReadOnlySpan<char> value)
    {
        value.CopyTo(_buffer[_position..]);
        _position += value.Length;
    }

    public void Append(char value)
    {
        _buffer[_position++] = value;
    }

    public readonly override string ToString() => new string(_buffer[.._position]);
}
