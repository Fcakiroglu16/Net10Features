# EF Core 10 Features Demonstration

This folder contains comprehensive examples of new features introduced in Entity Framework Core 10.

## Features Demonstrated

### 1. **Complex Type Support** (`ComplexTypeSupport.cs`)
Complex types are value objects stored in the same table as the owning entity.

**Key Points:**
- No separate table required
- No identity - pure value objects
- Always loaded with the owner entity
- Can be queried and filtered
- Perfect for: Address, Money, DateRange, etc.

**Example:**
```csharp
public class Product
{
    public Address ShippingAddress { get; set; }
    public Address BillingAddress { get; set; }
}

// Configuration
modelBuilder.Entity<Product>()
    .ComplexProperty(p => p.ShippingAddress);
```

**Benefits:**
- ? Better performance (no table joins)
- ? Cleaner schema
- ? Type safety
- ? Queryable properties

---

### 2. **Primitive Collections** (`PrimitiveCollections.cs`)
Store collections of primitive types directly in database columns.

**Key Points:**
- Store `List<int>`, `List<string>`, etc. without separate tables
- Stored as JSON or array in database
- Fully queryable with LINQ
- Supports Contains(), Count, and other operations

**Example:**
```csharp
public class Product
{
    public List<string> Tags { get; set; }
    public List<int> Ratings { get; set; }
}

// Query
var products = await context.Products
    .Where(p => p.Tags.Contains("electronics"))
    .ToListAsync();
```

**Benefits:**
- ? No junction tables needed
- ? Simpler schema
- ? Better performance for small collections
- ? Native LINQ support

---

### 3. **HierarchyId Support** (`HierarchyIdSupport.cs`)
Efficiently represent hierarchical tree structures.

**Key Points:**
- Path-based hierarchy representation (`/1/2/3/`)
- Fast ancestor/descendant queries
- No recursive CTEs needed
- Supports unlimited depth

**Example:**
```csharp
public class Category
{
    public string HierarchyPath { get; set; }
    public Category? Parent { get; set; }
    public List<Category> Children { get; set; }
}

// Find all descendants
var descendants = await context.Categories
    .Where(c => c.HierarchyPath.StartsWith("/1/"))
    .ToListAsync();
```

**Use Cases:**
- ?? File systems
- ?? Organizational charts
- ?? Category trees
- ?? Comment threads
- ??? Menu structures

---

### 4. **JSON Column Improvements** (`JsonColumnImprovements.cs`)
Enhanced support for storing and querying JSON data.

**Key Points:**
- Store complex objects as JSON
- Query nested properties with LINQ
- Update individual JSON properties
- Better indexing support in EF Core 10

**Example:**
```csharp
public class Product
{
    public ProductMetadata Metadata { get; set; }
}

public class ProductMetadata
{
    public string Description { get; set; }
    public Dictionary<string, string> Attributes { get; set; }
}

// Configuration
modelBuilder.Entity<Product>()
    .OwnsOne(p => p.Metadata, b => b.ToJson());

// Query
var products = await context.Products
    .Where(p => p.Metadata.Description.Contains("gaming"))
    .ToListAsync();
```

**Benefits:**
- ? Schema flexibility
- ? No migrations for attribute changes
- ? Queryable nested data
- ? Perfect for metadata and settings

---

### 5. **Raw SQL Improvements** (`RawSqlImprovements.cs`)
Enhanced raw SQL support with better composability.

**Key Points:**
- Compose LINQ on top of raw SQL
- Parameterized queries (SQL injection safe)
- Better stored procedure support
- ExecuteSql for bulk operations

**Example:**
```csharp
// Raw SQL with LINQ composition
var products = await context.Products
    .FromSql($"SELECT * FROM Products WHERE Price > {minPrice}")
    .Where(p => p.Name.Contains("Laptop"))
    .OrderBy(p => p.Price)
    .ToListAsync();

// Bulk update
await context.Database.ExecuteSql(
    $"UPDATE Products SET Price = Price * 1.1 WHERE Price < {1000}");
```

**Benefits:**
- ? Full SQL power when needed
- ? SQL injection protection
- ? Better bulk operation performance
- ? Database-specific features access

---

### 6. **Sentinel Values** (`SentinelValues.cs`)
Distinguish between "not set" and "set to default value".

**Key Points:**
- Use special value to indicate "not set" (e.g., -1)
- Different from nullable types
- Non-NULL database columns (better for indexes)
- EF Core change tracking respects sentinels

**Example:**
```csharp
public class Customer
{
    // -1 = not set, 0 = no discount, >0 = has discount
    public int DiscountPercentage { get; set; } = -1;
}

// Configuration
modelBuilder.Entity<Customer>()
    .Property(c => c.DiscountPercentage)
    .HasSentinel(-1);

// Business logic
if (customer.DiscountPercentage == -1)
{
    // Not configured yet - use default behavior
}
else if (customer.DiscountPercentage == 0)
{
    // Explicitly no discount
}
else
{
    // Apply discount
}
```

**Benefits:**
- ? Distinguish "not set" from zero/default
- ? Non-nullable columns (better indexing)
- ? No nullable type complexity
- ? Clear business semantics

**Use Cases:**
- ?? Discounts and pricing
- ?? Quotas and limits
- ?? Counters and statistics
- ?? Optional configuration values

---

## Database Context

The `AppDbContext.cs` file contains all EF Core 10 configurations:

```csharp
public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Customer> Customers { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Complex types, primitive collections, JSON columns, 
        // HierarchyId, and sentinel values configuration
    }
}
```

## Models

- **Product**: Demonstrates complex types, primitive collections, and JSON columns
- **Category**: Demonstrates HierarchyId for tree structures
- **Customer**: Demonstrates sentinel values

## Running the Demos

Each feature class has a `DemonstrateAsync()` method that:
1. Creates sample data
2. Performs various queries
3. Shows update operations
4. Explains benefits and use cases

All demos use an **InMemory database** for easy demonstration without requiring SQL Server setup.

## Key Takeaways

EF Core 10 focuses on:
- ?? **Developer productivity** - Less boilerplate code
- ? **Performance** - Better query generation and bulk operations
- ?? **Flexibility** - More mapping options for different scenarios
- ?? **Modern data patterns** - JSON, hierarchies, collections

## Comparison with Previous Versions

| Feature | Before EF Core 10 | EF Core 10 |
|---------|------------------|------------|
| Address types | Owned entities (separate table) or JSON | Complex types (same table) |
| Primitive lists | Separate junction table or JSON manual handling | Native primitive collections |
| Hierarchies | Recursive CTEs or manual path management | Built-in HierarchyId support |
| JSON queries | Limited, mostly client-side | Full server-side LINQ support |
| Raw SQL | Basic support | Full composability with LINQ |
| Default vs Not Set | Nullable types or workarounds | Sentinel values |

---

## Additional Resources

- [EF Core 10 Release Notes](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-10.0/whatsnew)
- [Complex Types Documentation](https://learn.microsoft.com/en-us/ef/core/modeling/complex-types)
- [Primitive Collections Documentation](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-8.0/whatsnew#primitive-collections)
- [JSON Columns Documentation](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0/whatsnew#json-columns)
