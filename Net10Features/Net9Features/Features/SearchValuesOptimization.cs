using System.Buffers;
using System.Diagnostics;

namespace Net10Features.Net9Features.Features;

/// <summary>
/// Demonstrates SearchValues optimization in .NET 9
/// SearchValues provides highly optimized searching for sets of values.
/// It uses SIMD acceleration and is much faster than Contains/Any for multiple searches.
/// </summary>
public class SearchValuesOptimization
{
    public static void Demonstrate()
    {
        Console.WriteLine("=== .NET 9: SearchValues Optimization ===\n");

        // 1. Basic SearchValues usage
        Console.WriteLine("1. Basic SearchValues for Characters\n");
        
        var vowels = SearchValues.Create("aeiouAEIOU");
        var text = "Hello World, this is a SearchValues demonstration!";
        
        Console.WriteLine($"   Text: {text}");
        Console.WriteLine($"   Searching for vowels...\n");
        
        var index = 0;
        while (index < text.Length)
        {
            var vowelIndex = text.AsSpan(index).IndexOfAny(vowels);
            if (vowelIndex == -1) break;
            
            var actualIndex = index + vowelIndex;
            Console.WriteLine($"   Found vowel '{text[actualIndex]}' at position {actualIndex}");
            index = actualIndex + 1;
        }

        // 2. SearchValues for strings
        Console.WriteLine("\n2. SearchValues for Strings\n");
        
        var badWords = SearchValues.Create(["spam", "scam", "fake", "fraud"], StringComparison.OrdinalIgnoreCase);
        var messages = new[]
        {
            "This is a legitimate message",
            "Click here for FAKE offers!",
            "Hello, this is a SCAM alert",
            "Normal conversation here",
            "Don't fall for this FRAUD"
        };

        Console.WriteLine("   Checking messages for inappropriate content:\n");
        foreach (var message in messages)
        {
            var containsBadWord = message.AsSpan().ContainsAny(badWords);
            var status = containsBadWord ? "? FLAGGED" : "? CLEAN";
            Console.WriteLine($"   {status}: {message}");
        }

        // 3. Digit detection
        Console.WriteLine("\n3. Detecting Digits in Strings\n");
        
        var digits = SearchValues.Create("0123456789");
        var inputs = new[]
        {
            "NoDigitsHere",
            "Product123",
            "Order#456789",
            "CustomerName"
        };

        foreach (var input in inputs)
        {
            var hasDigits = input.AsSpan().ContainsAny(digits);
            var digitCount = 0;
            
            if (hasDigits)
            {
                var span = input.AsSpan();
                var idx = 0;
                while (idx < span.Length)
                {
                    var digitIdx = span.Slice(idx).IndexOfAny(digits);
                    if (digitIdx == -1) break;
                    digitCount++;
                    idx += digitIdx + 1;
                }
            }

            Console.WriteLine($"   '{input}': {(hasDigits ? $"{digitCount} digit(s)" : "No digits")}");
        }

        // 4. Special characters validation
        Console.WriteLine("\n4. Password Strength Validation\n");
        
        var specialChars = SearchValues.Create("!@#$%^&*()_+-=[]{}|;:,.<>?");
        var uppercaseLetters = SearchValues.Create("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        var lowercaseLetters = SearchValues.Create("abcdefghijklmnopqrstuvwxyz");
        
        var passwords = new[]
        {
            "password",
            "Password123",
            "P@ssw0rd!",
            "ALLUPPERCASE",
            "Strong#Pass123"
        };

        foreach (var password in passwords)
        {
            var hasSpecial = password.AsSpan().ContainsAny(specialChars);
            var hasUpper = password.AsSpan().ContainsAny(uppercaseLetters);
            var hasLower = password.AsSpan().ContainsAny(lowercaseLetters);
            var hasDigit = password.AsSpan().ContainsAny(digits);
            var isLongEnough = password.Length >= 8;

            var strength = (hasSpecial ? 1 : 0) + (hasUpper ? 1 : 0) + 
                          (hasLower ? 1 : 0) + (hasDigit ? 1 : 0) + (isLongEnough ? 1 : 0);

            var strengthText = strength switch
            {
                5 => "Strong",
                3 or 4 => "Medium",
                _ => "Weak"
            };

            Console.WriteLine($"   '{password}':");
            Console.WriteLine($"     Strength: {strengthText}");
            Console.WriteLine($"     - Special chars: {hasSpecial}");
            Console.WriteLine($"     - Uppercase: {hasUpper}");
            Console.WriteLine($"     - Lowercase: {hasLower}");
            Console.WriteLine($"     - Digits: {hasDigit}");
            Console.WriteLine($"     - Length ?8: {isLongEnough}\n");
        }

        // 5. Performance comparison
        Console.WriteLine("5. Performance Comparison\n");
        
        var testText = string.Concat(Enumerable.Repeat("The quick brown fox jumps over the lazy dog. ", 10000));
        var vowelChars = new[] { 'a', 'e', 'i', 'o', 'u', 'A', 'E', 'I', 'O', 'U' };
        var searchVowels = SearchValues.Create(vowelChars);

        // Warm up
        _ = testText.AsSpan().IndexOfAny(searchVowels);
        _ = testText.AsSpan().IndexOfAny(vowelChars);

        // Benchmark SearchValues
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < 1000; i++)
        {
            var result = testText.AsSpan().IndexOfAny(searchVowels);
        }
        sw.Stop();
        var searchValuesTime = sw.Elapsed.TotalMilliseconds;

        // Benchmark traditional array
        sw.Restart();
        for (int i = 0; i < 1000; i++)
        {
            var result = testText.AsSpan().IndexOfAny(vowelChars);
        }
        sw.Stop();
        var arrayTime = sw.Elapsed.TotalMilliseconds;

        Console.WriteLine($"   Text length: {testText.Length:N0} characters");
        Console.WriteLine($"   Iterations: 1,000\n");
        Console.WriteLine($"   SearchValues: {searchValuesTime:F2}ms");
        Console.WriteLine($"   Char array: {arrayTime:F2}ms");
        Console.WriteLine($"   Improvement: {((arrayTime - searchValuesTime) / arrayTime * 100):F1}% faster");

        // 6. URL validation
        Console.WriteLine("\n6. URL Validation Example\n");
        
        var invalidUrlChars = SearchValues.Create(" <>\"{}|\\^`");
        var urls = new[]
        {
            "https://example.com/path",
            "https://example.com/path with spaces",
            "https://example.com/path<script>",
            "https://example.com/valid-url_123"
        };

        foreach (var url in urls)
        {
            var hasInvalidChars = url.AsSpan().ContainsAny(invalidUrlChars);
            var status = hasInvalidChars ? "? Invalid" : "? Valid";
            Console.WriteLine($"   {status}: {url}");
        }

        // 7. File extension filtering
        Console.WriteLine("\n7. File Extension Filtering\n");
        
        var imageExtensions = SearchValues.Create([".jpg", ".jpeg", ".png", ".gif", ".bmp"], StringComparison.OrdinalIgnoreCase);
        var files = new[]
        {
            "photo.jpg",
            "document.pdf",
            "image.PNG",
            "video.mp4",
            "picture.gif"
        };

        Console.WriteLine("   Image files:");
        foreach (var file in files)
        {
            var isImage = false;
            foreach (var ext in new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" })
            {
                if (file.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                {
                    isImage = true;
                    break;
                }
            }

            if (isImage)
            {
                Console.WriteLine($"   ?? {file}");
            }
        }

        // 8. Real-world: Log parsing
        Console.WriteLine("\n8. Real-World Example: Log Parsing\n");
        
        var errorKeywords = SearchValues.Create(["ERROR", "FATAL", "EXCEPTION", "FAILED"], StringComparison.OrdinalIgnoreCase);
        var warningKeywords = SearchValues.Create(["WARNING", "WARN"], StringComparison.OrdinalIgnoreCase);
        
        var logEntries = new[]
        {
            "[2024-01-01 10:00:00] INFO: Application started",
            "[2024-01-01 10:05:00] WARNING: High memory usage detected",
            "[2024-01-01 10:10:00] ERROR: Database connection failed",
            "[2024-01-01 10:15:00] INFO: Retry successful",
            "[2024-01-01 10:20:00] FATAL: Critical system failure"
        };

        var errorCount = 0;
        var warningCount = 0;

        foreach (var entry in logEntries)
        {
            var hasError = entry.AsSpan().ContainsAny(errorKeywords);
            var hasWarning = entry.AsSpan().ContainsAny(warningKeywords);

            if (hasError)
            {
                errorCount++;
                Console.WriteLine($"   ?? {entry}");
            }
            else if (hasWarning)
            {
                warningCount++;
                Console.WriteLine($"   ?? {entry}");
            }
            else
            {
                Console.WriteLine($"   ?? {entry}");
            }
        }

        Console.WriteLine($"\n   Summary: {errorCount} errors, {warningCount} warnings");

        // Key Benefits
        Console.WriteLine("\n?? SearchValues Benefits:");
        Console.WriteLine("   ? SIMD-accelerated searching");
        Console.WriteLine("   ? Up to 10x faster than Contains/Any");
        Console.WriteLine("   ? Works with chars, strings, bytes");
        Console.WriteLine("   ? Case-insensitive string searching");
        Console.WriteLine("   ? Optimized for repeated searches");
        Console.WriteLine("   ? Zero allocations after creation");
        Console.WriteLine("   ? Perfect for: validation, parsing, filtering");

        Console.WriteLine();
    }
}
