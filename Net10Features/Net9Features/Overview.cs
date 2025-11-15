// .NET 9 Features Overview
// This namespace demonstrates the key features introduced in .NET 9

namespace Net10Features.Net9Features;

/*
 * STRUCTURE:
 * 
 * Net9Features/
 * ??? Features/
 * ?   ??? LinqImprovements.cs              - New LINQ methods: CountBy, AggregateBy, Index
 * ?   ??? TimeProviderFeature.cs           - TimeProvider for testable time operations
 * ?   ??? SearchValuesOptimization.cs      - High-performance string/char searching
 * ?   ??? CollectionsMarshalFeatures.cs    - Low-level collection optimizations
 * ?   ??? TensorAndAIFeatures.cs           - Tensor<T> for AI/ML workloads
 * ?   ??? RegexImprovements.cs             - Source generation and performance improvements
 * ?
 * ??? Overview.cs                          - This file
 * ??? README.md                            - Comprehensive documentation
 * 
 * 
 * .NET 9 HIGHLIGHTS:
 * 
 * 1. LINQ IMPROVEMENTS (LinqImprovements.cs)
 *    - CountBy() - Count occurrences by key
 *    - AggregateBy() - Aggregate values by key
 *    - Index() - Get index with elements
 *    - Better performance and less allocations
 * 
 * 2. TIME PROVIDER (TimeProviderFeature.cs)
 *    - Abstraction over DateTime/DateTimeOffset
 *    - Testable time-dependent code
 *    - Built-in fake time provider
 *    - Timer and delay support
 * 
 * 3. SEARCH VALUES (SearchValuesOptimization.cs)
 *    - Optimized searching for sets of values
 *    - SIMD acceleration
 *    - Much faster than Contains/Any
 *    - For strings, chars, bytes
 * 
 * 4. COLLECTIONS MARSHAL (CollectionsMarshalFeatures.cs)
 *    - Low-level, high-performance operations
 *    - Direct memory access to collections
 *    - AsSpan() for List<T>
 *    - SetCount() without initialization
 * 
 * 5. TENSOR & AI (TensorAndAIFeatures.cs)
 *    - Tensor<T> for multi-dimensional arrays
 *    - SIMD-accelerated operations
 *    - AI/ML workload support
 *    - Integration with ML.NET
 * 
 * 6. REGEX IMPROVEMENTS (RegexImprovements.cs)
 *    - Source generators for compile-time regex
 *    - Better performance
 *    - Reduced allocations
 *    - Timeout support improvements
 * 
 * 
 * KEY BENEFITS OF .NET 9:
 * 
 * ? Performance improvements across the board
 * ? Better LINQ for data processing
 * ? Testable time abstractions
 * ? High-performance search operations
 * ? AI/ML support with Tensor<T>
 * ? Better regex compilation
 * ? Low-level optimization options
 * 
 * 
 * PERFORMANCE FOCUS:
 * 
 * .NET 9 is heavily focused on:
 * - SIMD acceleration
 * - Reduced allocations
 * - Better JIT compilation
 * - Native AOT improvements
 * - ARM64 optimizations
 */
