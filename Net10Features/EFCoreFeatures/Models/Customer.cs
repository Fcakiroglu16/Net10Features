namespace Net10Features.EFCoreFeatures.Models;

/// <summary>
/// Customer entity demonstrating Sentinel Values
/// </summary>
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    // Sentinel Values - EF Core 10 Feature
    // Allows distinguishing between "not set" and "set to default"
    // Default value -1 indicates "not set", while 0 is a valid value
    public int DiscountPercentage { get; set; } = -1;
    
    public DateTimeOffset? LastLoginDate { get; set; }
    public List<Order> Orders { get; set; } = new();
}

/// <summary>
/// Order entity
/// </summary>
public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
}
