using Net10Features.Features;

Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
Console.WriteLine("║      .NET 10 and C# 14 Features Demonstration             ║");
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

Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
Console.WriteLine("║              Demo Complete - Press Any Key                 ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════╝");

