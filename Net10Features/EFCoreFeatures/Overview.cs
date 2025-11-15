// Entity Framework Core 10 Features Overview
// This namespace demonstrates the key features introduced in EF Core 10

namespace Net10Features.EFCoreFeatures;

/*
 * STRUCTURE:
 * 
 * EFCoreFeatures/
 * ??? Models/
 * ?   ??? Product.cs         - Demonstrates Complex Types, Primitive Collections, JSON Columns
 * ?   ??? Category.cs        - Demonstrates HierarchyId for tree structures
 * ?   ??? Customer.cs        - Demonstrates Sentinel Values
 * ?
 * ??? Features/
 * ?   ??? ComplexTypeSupport.cs       - Value objects stored in same table
 * ?   ??? PrimitiveCollections.cs     - Collections of primitives without separate tables
 * ?   ??? HierarchyIdSupport.cs       - Hierarchical data (trees)
 * ?   ??? JsonColumnImprovements.cs   - Enhanced JSON querying
 * ?   ??? RawSqlImprovements.cs       - Better raw SQL and LINQ composition
 * ?   ??? SentinelValues.cs           - Distinguish "not set" from "default value"
 * ?
 * ??? AppDbContext.cs        - Database context with all EF Core 10 configurations
 * ??? README.md              - Comprehensive documentation
 * 
 * 
 * EF CORE 10 HIGHLIGHTS:
 * 
 * 1. COMPLEX TYPES (ComplexTypeSupport.cs)
 *    - Value objects without separate identity
 *    - Stored in same table as owner
 *    - Always loaded, no lazy loading overhead
 *    - Example: Address, Money, DateRange
 * 
 * 2. PRIMITIVE COLLECTIONS (PrimitiveCollections.cs)
 *    - Store List<int>, List<string> directly
 *    - No junction tables needed
 *    - Queryable with LINQ Contains, Count, etc.
 *    - Example: Tags, Ratings, Permissions
 * 
 * 3. HIERARCHYID (HierarchyIdSupport.cs)
 *    - Path-based hierarchy (/1/2/3/)
 *    - Fast ancestor/descendant queries
 *    - No recursive CTEs
 *    - Example: Org charts, Categories, File systems
 * 
 * 4. JSON COLUMNS (JsonColumnImprovements.cs)
 *    - Store complex objects as JSON
 *    - Query nested properties with LINQ
 *    - Schema flexibility
 *    - Example: Metadata, Settings, Flexible attributes
 * 
 * 5. RAW SQL (RawSqlImprovements.cs)
 *    - Compose LINQ on top of raw SQL
 *    - Better parameter handling
 *    - ExecuteSql for bulk operations
 *    - Example: Complex queries, Stored procedures
 * 
 * 6. SENTINEL VALUES (SentinelValues.cs)
 *    - Special value indicating "not set"
 *    - Different from nullable types
 *    - Non-NULL database columns
 *    - Example: -1 for not set, 0 for no discount, >0 for discount
 * 
 * 
 * USAGE:
 * 
 * Each feature class has a DemonstrateAsync() method:
 * 
 *   await ComplexTypeSupport.DemonstrateAsync();
 *   await PrimitiveCollections.DemonstrateAsync();
 *   await HierarchyIdSupport.DemonstrateAsync();
 *   await JsonColumnImprovements.DemonstrateAsync();
 *   await RawSqlImprovements.DemonstrateAsync();
 *   await SentinelValues.DemonstrateAsync();
 * 
 * 
 * DATABASE:
 * 
 * Uses InMemory database for easy demonstration.
 * To use SQL Server:
 * 
 *   optionsBuilder.UseSqlServer("Server=...;Database=EFCore10Demo;...");
 * 
 * 
 * KEY BENEFITS OF EF CORE 10:
 * 
 * ? Less boilerplate code
 * ? Better performance
 * ? More flexible mapping options
 * ? Modern data patterns support
 * ? Simpler schema design
 * ? Enhanced querying capabilities
 */
