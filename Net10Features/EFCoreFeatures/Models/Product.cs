namespace Net10Features.EFCoreFeatures.Models;

/// <summary>
/// Product entity demonstrating EF Core 10 features
/// </summary>
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    
    // Primitive Collections - EF Core 10 Feature
    // Store collections of primitive types directly in the database
    public List<string> Tags { get; set; } = new();
    public List<int> Ratings { get; set; } = new();
    
    // Complex Type Support - EF Core 10 Feature
    public Address ShippingAddress { get; set; } = new();
    public Address BillingAddress { get; set; } = new();
    
    // JSON Column - EF Core 7+ (Improved in EF Core 10)
    public ProductMetadata Metadata { get; set; } = new();
}

/// <summary>
/// Complex type for addresses - stored as part of the Product table
/// </summary>
public class Address
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}

/// <summary>
/// Metadata stored as JSON column
/// </summary>
public class ProductMetadata
{
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, string> Attributes { get; set; } = new();
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
