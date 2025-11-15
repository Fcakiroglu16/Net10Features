using System.Numerics.Tensors;

namespace Net10Features.Net9Features.Features;

/// <summary>
/// Demonstrates Tensor and AI features in .NET 9
/// TensorPrimitives provides high-performance SIMD-accelerated operations for AI/ML workloads.
/// </summary>
public class TensorAndAIFeatures
{
    public static void Demonstrate()
    {
        Console.WriteLine("=== .NET 9: TensorPrimitives and AI Features ===\n");

        // 1. Basic vector operations
        Console.WriteLine("1. Basic Vector Operations (SIMD-Accelerated)\n");
        
        var vectorA = new float[] { 1, 2, 3, 4, 5 };
        var vectorB = new float[] { 5, 4, 3, 2, 1 };
        var result = new float[5];

        Console.WriteLine($"   Vector A: [{string.Join(", ", vectorA)}]");
        Console.WriteLine($"   Vector B: [{string.Join(", ", vectorB)}]");

        // Addition
        TensorPrimitives.Add(vectorA, vectorB, result);
        Console.WriteLine($"   A + B: [{string.Join(", ", result)}]");

        // Multiplication
        TensorPrimitives.Multiply(vectorA, vectorB, result);
        Console.WriteLine($"   A * B: [{string.Join(", ", result)}]");

        // 2. Statistical operations
        Console.WriteLine("\n2. Statistical Operations\n");
        
        var data = new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        
        Console.WriteLine($"   Data: [{string.Join(", ", data)}]");
        
        var sum = TensorPrimitives.Sum(data);
        var max = TensorPrimitives.Max(data);
        var min = TensorPrimitives.Min(data);
        var mean = sum / data.Length;

        Console.WriteLine($"   Sum: {sum}");
        Console.WriteLine($"   Mean: {mean}");
        Console.WriteLine($"   Max: {max}");
        Console.WriteLine($"   Min: {min}");

        // 3. Dot product
        Console.WriteLine("\n3. Dot Product (SIMD-Accelerated)\n");

        var vec1 = new float[] { 1, 2, 3 };
        var vec2 = new float[] { 4, 5, 6 };

        var dotProduct = TensorPrimitives.Dot(vec1, vec2);
        
        Console.WriteLine($"   Vector 1: [{string.Join(", ", vec1)}]");
        Console.WriteLine($"   Vector 2: [{string.Join(", ", vec2)}]");
        Console.WriteLine($"   Dot product: {dotProduct}");
        Console.WriteLine($"   Formula: (1*4) + (2*5) + (3*6) = {dotProduct}");

        // 4. Activation functions for neural networks
        Console.WriteLine("\n4. Neural Network Activation Functions\n");

        var inputs = new float[] { -2f, -1f, 0f, 1f, 2f };
        Console.WriteLine($"   Input: [{string.Join(", ", inputs)}]");

        // Sigmoid
        var sigmoid = new float[inputs.Length];
        TensorPrimitives.Sigmoid(inputs, sigmoid);
        Console.WriteLine($"   Sigmoid: [{string.Join(", ", sigmoid.Select(x => $"{x:F4}"))}]");

        // Tanh
        var tanh = new float[inputs.Length];
        TensorPrimitives.Tanh(inputs, tanh);
        Console.WriteLine($"   Tanh: [{string.Join(", ", tanh.Select(x => $"{x:F4}"))}]");

        // Exp (exponential)
        var exp = new float[inputs.Length];
        TensorPrimitives.Exp(inputs, exp);
        Console.WriteLine($"   Exp: [{string.Join(", ", exp.Select(x => $"{x:F2}"))}]");

        // 5. Distance calculations
        Console.WriteLine("\n5. Distance Calculations (for ML)\n");

        var point1 = new float[] { 1, 2, 3 };
        var point2 = new float[] { 4, 5, 6 };

        Console.WriteLine($"   Point 1: [{string.Join(", ", point1)}]");
        Console.WriteLine($"   Point 2: [{string.Join(", ", point2)}]");

        // Euclidean distance
        var distance = TensorPrimitives.Distance(point1, point2);
        Console.WriteLine($"   Euclidean distance: {distance:F4}");

        // Cosine similarity
        var cosineSimilarity = TensorPrimitives.CosineSimilarity(point1, point2);
        Console.WriteLine($"   Cosine similarity: {cosineSimilarity:F4}");

        // 6. Normalization
        Console.WriteLine("\n6. Vector Normalization\n");

        var vectorToNormalize = new float[] { 3, 4 };
        var normalized = new float[2];

        Console.WriteLine($"   Original vector: [{string.Join(", ", vectorToNormalize)}]");
        
        var norm = TensorPrimitives.Norm(vectorToNormalize);
        Console.WriteLine($"   Norm (length): {norm}");

        // Normalize
        for (int i = 0; i < vectorToNormalize.Length; i++)
        {
            normalized[i] = vectorToNormalize[i] / norm;
        }
        
        Console.WriteLine($"   Normalized: [{string.Join(", ", normalized.Select(x => $"{x:F4}"))}]");

        // 7. Performance comparison
        Console.WriteLine("\n7. Performance: SIMD vs Regular Loop\n");

        var largeArray = new float[100000];
        for (int i = 0; i < largeArray.Length; i++)
        {
            largeArray[i] = i * 0.1f;
        }

        var result1 = new float[largeArray.Length];
        var result2 = new float[largeArray.Length];

        var sw = System.Diagnostics.Stopwatch.StartNew();
        
        // Regular loop
        for (int i = 0; i < largeArray.Length; i++)
        {
            result1[i] = MathF.Sqrt(largeArray[i]);
        }
        sw.Stop();
        var regularTime = sw.Elapsed.TotalMilliseconds;

        // SIMD-accelerated
        sw.Restart();
        TensorPrimitives.Sqrt(largeArray, result2);
        sw.Stop();
        var simdTime = sw.Elapsed.TotalMilliseconds;

        Console.WriteLine($"   Array size: {largeArray.Length:N0}");
        Console.WriteLine($"   Regular loop: {regularTime:F3}ms");
        Console.WriteLine($"   SIMD (TensorPrimitives): {simdTime:F3}ms");
        Console.WriteLine($"   Speedup: {regularTime / simdTime:F1}x faster");

        // 8. Softmax (used in neural networks)
        Console.WriteLine("\n8. Softmax Function (Neural Network Output Layer)\n");

        var logits = new float[] { 2.0f, 1.0f, 0.1f };
        var softmax = new float[logits.Length];

        Console.WriteLine($"   Logits: [{string.Join(", ", logits)}]");

        TensorPrimitives.SoftMax(logits, softmax);

        Console.WriteLine($"   Softmax: [{string.Join(", ", softmax.Select(x => $"{x:F4}"))}]");
        Console.WriteLine($"   Sum of probabilities: {softmax.Sum():F4} (should be ~1.0)");

        // 9. Element-wise operations
        Console.WriteLine("\n9. Element-Wise Operations\n");

        var values = new float[] { 1, 4, 9, 16, 25 };
        var sqrtResults = new float[values.Length];
        var squareResults = new float[values.Length];

        Console.WriteLine($"   Values: [{string.Join(", ", values)}]");

        // Square root
        TensorPrimitives.Sqrt(values, sqrtResults);
        Console.WriteLine($"   Square roots: [{string.Join(", ", sqrtResults)}]");

        // Square (multiply by itself)
        TensorPrimitives.Multiply(values, values, squareResults);
        Console.WriteLine($"   Squared: [{string.Join(", ", squareResults)}]");

        // 10. Real-world example: Batch normalization
        Console.WriteLine("\n10. Real-World Example: Batch Normalization\n");

        var batch = new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var batchMean = TensorPrimitives.Sum(batch) / batch.Length;
        
        // Calculate variance
        var diffs = new float[batch.Length];
        var diffSquared = new float[batch.Length];
        
        for (int i = 0; i < batch.Length; i++)
        {
            diffs[i] = batch[i] - batchMean;
        }
        
        TensorPrimitives.Multiply(diffs, diffs, diffSquared);
        var variance = TensorPrimitives.Sum(diffSquared) / batch.Length;
        var stdDev = MathF.Sqrt(variance);

        Console.WriteLine($"   Batch: [{string.Join(", ", batch)}]");
        Console.WriteLine($"   Mean: {batchMean:F2}");
        Console.WriteLine($"   Variance: {variance:F2}");
        Console.WriteLine($"   Std Dev: {stdDev:F2}");

        // Normalize
        var normalized2 = new float[batch.Length];
        for (int i = 0; i < batch.Length; i++)
        {
            normalized2[i] = (batch[i] - batchMean) / stdDev;
        }

        Console.WriteLine($"   Normalized: [{string.Join(", ", normalized2.Select(x => $"{x:F2}"))}]");

        // Key Benefits
        Console.WriteLine("\n?? TensorPrimitives and AI Features Benefits:");
        Console.WriteLine("   ? SIMD-accelerated operations");
        Console.WriteLine("   ? 10-100x faster than regular loops");
        Console.WriteLine("   ? Built-in ML/AI primitives");
        Console.WriteLine("   ? Distance and similarity calculations");
        Console.WriteLine("   ? Activation functions (Sigmoid, Tanh, SoftMax)");
        Console.WriteLine("   ? Statistical operations (Sum, Mean, Max, Min)");
        Console.WriteLine("   ? Hardware acceleration support");
        Console.WriteLine("   ? Perfect for: ML/AI, data processing, scientific computing");

        Console.WriteLine();
    }
}
