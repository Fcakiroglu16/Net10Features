using System.Text.RegularExpressions;

namespace Net10Features.Net9Features.Features;

/// <summary>
/// Demonstrates Regex improvements in .NET 9
/// Includes source generators for compile-time regex, better performance,
/// and reduced allocations.
/// </summary>
public partial class RegexImprovements
{
    // Source-generated regex (compile-time)
    // Much faster than runtime compilation
    [GeneratedRegex(@"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}\b", RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"^\d{3}-\d{2}-\d{4}$")]
    private static partial Regex SsnRegex();

    [GeneratedRegex(@"^(\+\d{1,3})?[\s-]?\(?\d{3}\)?[\s-]?\d{3}[\s-]?\d{4}$")]
    private static partial Regex PhoneRegex();

    [GeneratedRegex(@"https?://[^\s]+")]
    private static partial Regex UrlRegex();

    [GeneratedRegex(@"\b\w{3,}\b")]
    private static partial Regex WordRegex();

    public static void Demonstrate()
    {
        Console.WriteLine("=== .NET 9: Regex Improvements ===\n");

        // 1. Source-generated regex
        Console.WriteLine("1. Source-Generated Regex (Compile-Time)\n");
        
        var emails = new[]
        {
            "valid.email@example.com",
            "invalid.email@",
            "another@valid.co.uk",
            "notanemail"
        };

        Console.WriteLine("   Validating emails with source-generated regex:");
        foreach (var email in emails)
        {
            var isValid = EmailRegex().IsMatch(email);
            var status = isValid ? "?" : "?";
            Console.WriteLine($"   {status} {email}");
        }

        Console.WriteLine("\n   Benefits of source-generated regex:");
        Console.WriteLine("   - Compiled at build time, not runtime");
        Console.WriteLine("   - Zero startup overhead");
        Console.WriteLine("   - Better performance");
        Console.WriteLine("   - Errors caught at compile time");

        // 2. SSN validation
        Console.WriteLine("\n2. Social Security Number Validation\n");
        
        var ssns = new[]
        {
            "123-45-6789",
            "987-65-4321",
            "12-345-6789",
            "123456789"
        };

        foreach (var ssn in ssns)
        {
            var isValid = SsnRegex().IsMatch(ssn);
            var status = isValid ? "?" : "?";
            Console.WriteLine($"   {status} {ssn}");
        }

        // 3. Phone number validation
        Console.WriteLine("\n3. Phone Number Validation\n");
        
        var phones = new[]
        {
            "123-456-7890",
            "(123) 456-7890",
            "+1 123 456 7890",
            "1234567890",
            "123-45-6789"
        };

        foreach (var phone in phones)
        {
            var isValid = PhoneRegex().IsMatch(phone);
            var status = isValid ? "?" : "?";
            Console.WriteLine($"   {status} {phone}");
        }

        // 4. URL extraction
        Console.WriteLine("\n4. URL Extraction from Text\n");
        
        var text = "Check out https://example.com and http://test.org for more info!";
        Console.WriteLine($"   Text: {text}\n");

        var urlMatches = UrlRegex().Matches(text);
        Console.WriteLine($"   Found {urlMatches.Count} URL(s):");
        foreach (Match m in urlMatches)
        {
            Console.WriteLine($"   - {m.Value}");
        }

        // 5. Performance comparison
        Console.WriteLine("\n5. Performance: Source-Generated vs Runtime Compiled\n");

        var testEmail = "test@example.com";
        
        // Runtime compiled regex
        var runtimeRegex = new Regex(@"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}\b", 
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        var sw = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < 10000; i++)
        {
            _ = runtimeRegex.IsMatch(testEmail);
        }
        sw.Stop();
        var runtimeTime = sw.Elapsed.TotalMilliseconds;

        // Source-generated regex
        sw.Restart();
        for (int i = 0; i < 10000; i++)
        {
            _ = EmailRegex().IsMatch(testEmail);
        }
        sw.Stop();
        var generatedTime = sw.Elapsed.TotalMilliseconds;

        Console.WriteLine($"   Iterations: 10,000");
        Console.WriteLine($"   Runtime compiled: {runtimeTime:F2}ms");
        Console.WriteLine($"   Source-generated: {generatedTime:F2}ms");
        Console.WriteLine($"   Improvement: {((runtimeTime - generatedTime) / runtimeTime * 100):F1}% faster");

        // 6. Word extraction with counts
        Console.WriteLine("\n6. Word Extraction and Frequency\n");
        
        var paragraph = "The quick brown fox jumps over the lazy dog. The fox was very quick and the dog was lazy.";
        Console.WriteLine($"   Text: {paragraph}\n");

        var wordMatches = WordRegex().Matches(paragraph.ToLower());
        var wordCounts = new Dictionary<string, int>();

        foreach (Match m in wordMatches)
        {
            var word = m.Value;
            wordCounts[word] = wordCounts.GetValueOrDefault(word) + 1;
        }

        Console.WriteLine("   Word frequency (3+ characters):");
        foreach (var (word, count) in wordCounts.OrderByDescending(x => x.Value))
        {
            Console.WriteLine($"   - {word}: {count}");
        }

        // 7. Replace operations
        Console.WriteLine("\n7. Replace Operations\n");
        
        var html = "<p>Hello <b>World</b>! Visit <a href='#'>here</a>.</p>";
        Console.WriteLine($"   Original HTML: {html}");

        // Remove HTML tags
        var noTagsRegex = new Regex("<[^>]+>");
        var plainText = noTagsRegex.Replace(html, "");
        Console.WriteLine($"   Plain text: {plainText}");

        // 8. Split operations
        Console.WriteLine("\n8. Split Operations\n");
        
        var csv = "John,Doe,30,Engineer;Jane,Smith,28,Designer;Bob,Jones,35,Manager";
        Console.WriteLine($"   CSV: {csv}\n");

        var splitRegex = new Regex("[,;]");
        var fields = splitRegex.Split(csv);
        
        Console.WriteLine("   Parsed fields:");
        for (int i = 0; i < fields.Length; i += 4)
        {
            if (i + 3 < fields.Length)
            {
                Console.WriteLine($"   - {fields[i]} {fields[i + 1]}, Age: {fields[i + 2]}, Role: {fields[i + 3]}");
            }
        }

        // 9. Named groups
        Console.WriteLine("\n9. Named Groups\n");
        
        var dateRegex = new Regex(@"(?<year>\d{4})-(?<month>\d{2})-(?<day>\d{2})");
        var date = "2024-01-15";
        
        var dateMatch = dateRegex.Match(date);
        if (dateMatch.Success)
        {
            Console.WriteLine($"   Date: {date}");
            Console.WriteLine($"   Year: {dateMatch.Groups["year"].Value}");
            Console.WriteLine($"   Month: {dateMatch.Groups["month"].Value}");
            Console.WriteLine($"   Day: {dateMatch.Groups["day"].Value}");
        }

        // 10. Timeout support
        Console.WriteLine("\n10. Regex Timeout (Safety Feature)\n");
        
        try
        {
            // Potentially catastrophic backtracking pattern
            var dangerousPattern = @"(a+)+$";
            var dangerousInput = new string('a', 20) + "b";
            
            var timeoutRegex = new Regex(dangerousPattern, RegexOptions.None, TimeSpan.FromMilliseconds(100));
            
            Console.WriteLine("   Testing with timeout protection...");
            var sw2 = System.Diagnostics.Stopwatch.StartNew();
            
            try
            {
                _ = timeoutRegex.IsMatch(dangerousInput);
                sw2.Stop();
                Console.WriteLine($"   Completed in {sw2.ElapsedMilliseconds}ms");
            }
            catch (RegexMatchTimeoutException)
            {
                sw2.Stop();
                Console.WriteLine($"   ??  Timeout after {sw2.ElapsedMilliseconds}ms (protected from catastrophic backtracking)");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Error: {ex.Message}");
        }

        // 11. Real-world example: Log parsing
        Console.WriteLine("\n11. Real-World Example: Log Parsing\n");

        var logRegex = new Regex(@"\[(?<timestamp>[^\]]+)\]\s+(?<level>\w+):\s+(?<message>.+)");
        var logs = new[]
        {
            "[2024-01-15 10:30:00] INFO: Application started",
            "[2024-01-15 10:30:05] WARNING: High memory usage",
            "[2024-01-15 10:30:10] ERROR: Database connection failed"
        };

        Console.WriteLine("   Parsing log entries:\n");
        foreach (var log in logs)
        {
            var logMatch = logRegex.Match(log);
            if (logMatch.Success)
            {
                var level = logMatch.Groups["level"].Value;
                var icon = level switch
                {
                    "INFO" => "??",
                    "WARNING" => "??",
                    "ERROR" => "??",
                    _ => "?"
                };

                Console.WriteLine($"   {icon} [{logMatch.Groups["timestamp"].Value}]");
                Console.WriteLine($"      Level: {level}");
                Console.WriteLine($"      Message: {logMatch.Groups["message"].Value}\n");
            }
        }

        // Key Benefits
        Console.WriteLine("?? Regex Improvements Benefits:");
        Console.WriteLine("   ? Source generation - compile-time regex");
        Console.WriteLine("   ? Zero startup overhead");
        Console.WriteLine("   ? Better performance (up to 2x faster)");
        Console.WriteLine("   ? Reduced allocations");
        Console.WriteLine("   ? Compile-time error detection");
        Console.WriteLine("   ? Timeout support for safety");
        Console.WriteLine("   ? Named groups for readability");
        Console.WriteLine("   ? Perfect for: validation, parsing, extraction");

        Console.WriteLine();
    }
}
