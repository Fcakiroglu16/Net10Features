using Net10Features.Features;
using Net10Features.EFCoreFeatures.Features;

Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
Console.WriteLine("║      .NET 10 and C# 14 Features Demonstration             ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
Console.WriteLine();

// ========== C# 14 / .NET 10 Language Features ==========
Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
Console.WriteLine("║         C# 14 / .NET 10 Language Features                  ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
Console.WriteLine();

// 1. Collection Expression Improvements
await Task.Run(() => CollectionExpressionImprovements.Demonstrate());

// 2. Params Collection Enhancements
await Task.Run(() => ParamsCollectionEnhancements.Demonstrate());

// 3. LINQ Improvements
await Task.Run(() => LinqImprovements.Demonstrate());

// 4. Async/Await Improvements
await AsyncImprovements.DemonstrateAsync();

// 5. Overload Resolution Improvements
await Task.Run(() => OverloadResolutionImprovements.Demonstrate());

// 6. Primary Constructors in Structs
await Task.Run(() => PrimaryConstructorsInStructs.Demonstrate());

// 7. Field Keyword in Properties
await Task.Run(() => FieldKeywordInProperties.Demonstrate());

// 8. Ref Struct Enhancements
await Task.Run(() => RefStructEnhancements.Demonstrate());

// ========== EF Core 10 Features ==========
Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
Console.WriteLine("║              EF Core 10 Features                           ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
Console.WriteLine();

// 1. Complex Type Support
await ComplexTypeSupport.DemonstrateAsync();

// 2. Primitive Collections
await PrimitiveCollections.DemonstrateAsync();

// 3. HierarchyId Support
await HierarchyIdSupport.DemonstrateAsync();

// 4. JSON Column Improvements
await JsonColumnImprovements.DemonstrateAsync();

// 5. Raw SQL Improvements
await RawSqlImprovements.DemonstrateAsync();

// 6. Sentinel Values
await SentinelValues.DemonstrateAsync();

Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
Console.WriteLine("║              Demo Complete - Press Any Key                 ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════╝");

