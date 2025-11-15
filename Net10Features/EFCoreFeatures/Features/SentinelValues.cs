using Microsoft.EntityFrameworkCore;
using Net10Features.EFCoreFeatures.Models;

namespace Net10Features.EFCoreFeatures.Features;

/// <summary>
/// Demonstrates Sentinel Values in EF Core 10
/// Sentinel values allow distinguishing between "not set" and "set to default value".
/// This is important for properties where the default value (like 0) is a valid value.
/// </summary>
public class SentinelValues
{
    public static async Task DemonstrateAsync()
    {
        Console.WriteLine("=== EF Core 10: Sentinel Values ===\n");

        using var context = new AppDbContext();
        await context.Database.EnsureCreatedAsync();

        // 1. Understanding the problem
        Console.WriteLine("1. The Problem: Distinguishing 'not set' from 'zero'");
        Console.WriteLine("   Traditional approach: 0 could mean 'not set' OR 'no discount'");
        Console.WriteLine("   Sentinel value approach: -1 means 'not set', 0+ are valid values\n");

        // 2. Create customer with sentinel value (not set)
        Console.WriteLine("2. Creating customer WITHOUT setting discount...");
        
        var customer1 = new Customer
        {
            Name = "Alice Johnson",
            Email = "alice@example.com"
            // DiscountPercentage not set, defaults to -1 (sentinel value)
        };

        context.Customers.Add(customer1);
        await context.SaveChangesAsync();

        Console.WriteLine($"   Customer: {customer1.Name}");
        Console.WriteLine($"   DiscountPercentage: {customer1.DiscountPercentage}");
        Console.WriteLine($"   Status: {(customer1.DiscountPercentage == -1 ? "NOT SET (sentinel)" : "SET")}");

        // 3. Create customer with explicit zero discount
        Console.WriteLine("\n3. Creating customer WITH zero discount (explicitly set)...");
        
        var customer2 = new Customer
        {
            Name = "Bob Smith",
            Email = "bob@example.com",
            DiscountPercentage = 0 // Explicitly set to 0 - no discount
        };

        context.Customers.Add(customer2);
        await context.SaveChangesAsync();

        Console.WriteLine($"   Customer: {customer2.Name}");
        Console.WriteLine($"   DiscountPercentage: {customer2.DiscountPercentage}");
        Console.WriteLine($"   Status: {(customer2.DiscountPercentage == -1 ? "NOT SET (sentinel)" : "SET")}");
        Console.WriteLine($"   Meaning: Customer explicitly gets NO discount (0%)");

        // 4. Create customer with actual discount
        Console.WriteLine("\n4. Creating customer WITH 15% discount...");
        
        var customer3 = new Customer
        {
            Name = "Charlie Brown",
            Email = "charlie@example.com",
            DiscountPercentage = 15 // VIP customer with 15% discount
        };

        context.Customers.Add(customer3);
        await context.SaveChangesAsync();

        Console.WriteLine($"   Customer: {customer3.Name}");
        Console.WriteLine($"   DiscountPercentage: {customer3.DiscountPercentage}%");
        Console.WriteLine($"   Status: {(customer3.DiscountPercentage == -1 ? "NOT SET (sentinel)" : "SET")}");

        // 5. Query customers by sentinel status
        Console.WriteLine("\n5. Querying customers by discount status...");
        
        var customersWithoutDiscount = await context.Customers
            .Where(c => c.DiscountPercentage == -1)
            .ToListAsync();

        var customersWithZeroDiscount = await context.Customers
            .Where(c => c.DiscountPercentage == 0)
            .ToListAsync();

        var customersWithDiscount = await context.Customers
            .Where(c => c.DiscountPercentage > 0)
            .ToListAsync();

        Console.WriteLine($"   Customers with unset discount (sentinel -1): {customersWithoutDiscount.Count}");
        foreach (var c in customersWithoutDiscount)
        {
            Console.WriteLine($"   - {c.Name}: Discount not configured yet");
        }

        Console.WriteLine($"\n   Customers with ZERO discount (explicitly set): {customersWithZeroDiscount.Count}");
        foreach (var c in customersWithZeroDiscount)
        {
            Console.WriteLine($"   - {c.Name}: No discount (by design)");
        }

        Console.WriteLine($"\n   Customers with active discount: {customersWithDiscount.Count}");
        foreach (var c in customersWithDiscount)
        {
            Console.WriteLine($"   - {c.Name}: {c.DiscountPercentage}% discount");
        }

        // 6. Update sentinel value to actual value
        Console.WriteLine("\n6. Updating customer from sentinel to actual discount...");
        
        var customerToUpdate = await context.Customers
            .FirstAsync(c => c.Name == "Alice Johnson");

        Console.WriteLine($"   Before: {customerToUpdate.Name} - DiscountPercentage = {customerToUpdate.DiscountPercentage}");
        
        customerToUpdate.DiscountPercentage = 10; // Set 10% discount
        await context.SaveChangesAsync();

        Console.WriteLine($"   After: {customerToUpdate.Name} - DiscountPercentage = {customerToUpdate.DiscountPercentage}%");
        Console.WriteLine($"   Customer now has a valid discount configured!");

        // 7. Business logic using sentinel values
        Console.WriteLine("\n7. Business logic: Calculate final price with discount...");
        
        var allCustomers = await context.Customers.ToListAsync();
        var orderAmount = 100m;

        foreach (var customer in allCustomers)
        {
            decimal finalPrice;
            string explanation;

            if (customer.DiscountPercentage == -1)
            {
                // Sentinel value - use default behavior (no discount)
                finalPrice = orderAmount;
                explanation = "No discount policy set - using full price";
            }
            else if (customer.DiscountPercentage == 0)
            {
                // Explicitly set to zero - customer gets no discount
                finalPrice = orderAmount;
                explanation = "Explicitly set to no discount";
            }
            else
            {
                // Customer has a discount
                finalPrice = orderAmount * (1 - customer.DiscountPercentage / 100m);
                explanation = $"{customer.DiscountPercentage}% discount applied";
            }

            Console.WriteLine($"   {customer.Name}:");
            Console.WriteLine($"   - Order Amount: ${orderAmount}");
            Console.WriteLine($"   - {explanation}");
            Console.WriteLine($"   - Final Price: ${finalPrice:F2}\n");
        }

        // 8. Use case comparison
        Console.WriteLine("8. Sentinel Values vs Nullable Types:\n");
        
        Console.WriteLine("   Nullable int? (DiscountPercentage?):");
        Console.WriteLine("   ? Can distinguish null (not set) from 0 (no discount)");
        Console.WriteLine("   ? Database column must allow NULL");
        Console.WriteLine("   ? Nullable reference warnings in C#");
        
        Console.WriteLine("\n   Sentinel Values (int with -1 as sentinel):");
        Console.WriteLine("   ? Can distinguish sentinel (-1) from 0 (no discount)");
        Console.WriteLine("   ? Database column is NOT NULL (better for indexes)");
        Console.WriteLine("   ? No nullable type complexity");
        Console.WriteLine("   ? EF Core tracks sentinel value properly");

        // Key Benefits of Sentinel Values:
        Console.WriteLine("\n?? Sentinel Values Benefits:");
        Console.WriteLine("   ? Distinguish 'not set' from 'default value'");
        Console.WriteLine("   ? Non-nullable database columns (better performance)");
        Console.WriteLine("   ? Avoid nullable type complexity");
        Console.WriteLine("   ? EF Core change tracking respects sentinels");
        Console.WriteLine("   ? Better for indexing (no NULL values)");
        Console.WriteLine("   ? Clear business semantics");
        Console.WriteLine("   ? Useful for: discounts, quotas, limits, counters");
        Console.WriteLine("   ??  Choose sentinel value outside valid range (e.g., -1, int.MinValue)");

        Console.WriteLine();
    }
}
