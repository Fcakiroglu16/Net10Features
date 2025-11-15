namespace Net10Features.EFCoreFeatures.Models;

/// <summary>
/// Category entity demonstrating hierarchical data with HierarchyId
/// </summary>
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    // HierarchyId support - EF Core 10 Feature
    // Efficiently represents hierarchical data (tree structures)
    // Example: /1/ -> /1/1/ -> /1/1/1/
    public string HierarchyPath { get; set; } = "/";
    
    public int? ParentId { get; set; }
    public Category? Parent { get; set; }
    public List<Category> Children { get; set; } = new();
}
