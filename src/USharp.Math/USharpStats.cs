namespace USharp.Math;

/// <summary>
/// Provides statistical computation functions for U# programs.
/// </summary>
public static class USharpStats
{
    /// <summary>Computes the sum of a sequence.</summary>
    public static double Sum(IEnumerable<double> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return values.Sum();
    }

    /// <summary>Computes the arithmetic mean of a sequence.</summary>
    public static double Mean(IEnumerable<double> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        var list = values.ToList();
        if (list.Count == 0)
            throw new InvalidOperationException("Sequence contains no elements.");
        return list.Sum() / list.Count;
    }

    /// <summary>Computes the median of a sequence.</summary>
    public static double Median(IEnumerable<double> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        var sorted = values.OrderBy(v => v).ToList();
        if (sorted.Count == 0)
            throw new InvalidOperationException("Sequence contains no elements.");
        int mid = sorted.Count / 2;
        return sorted.Count % 2 == 0
            ? (sorted[mid - 1] + sorted[mid]) / 2.0
            : sorted[mid];
    }

    /// <summary>Computes the variance of a sequence (population variance).</summary>
    public static double Variance(IEnumerable<double> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        var list = values.ToList();
        if (list.Count == 0)
            throw new InvalidOperationException("Sequence contains no elements.");
        double mean = Mean(list);
        return list.Average(v => System.Math.Pow(v - mean, 2));
    }

    /// <summary>Computes the standard deviation of a sequence.</summary>
    public static double StdDev(IEnumerable<double> values) =>
        System.Math.Sqrt(Variance(values));

    /// <summary>Returns the minimum value in a sequence.</summary>
    public static double Min(IEnumerable<double> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return values.Min();
    }

    /// <summary>Returns the maximum value in a sequence.</summary>
    public static double Max(IEnumerable<double> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return values.Max();
    }

    /// <summary>Returns the range (max − min) of a sequence.</summary>
    public static double Range(IEnumerable<double> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        var list = values.ToList();
        return Max(list) - Min(list);
    }
}

/// <summary>
/// A simple pseudo-random number generator for U# programs.
/// </summary>
public sealed class USharpRandom
{
    private readonly Random _rng;

    /// <summary>Creates a new instance with a random seed.</summary>
    public USharpRandom() => _rng = new Random();

    /// <summary>Creates a new instance with the specified seed.</summary>
    public USharpRandom(int seed) => _rng = new Random(seed);

    /// <summary>Returns a random double in [0, 1).</summary>
    public double NextDouble() => _rng.NextDouble();

    /// <summary>Returns a random integer in [0, <paramref name="maxExclusive"/>).</summary>
    public int NextInt(int maxExclusive) => _rng.Next(maxExclusive);

    /// <summary>Returns a random integer in [<paramref name="min"/>, <paramref name="max"/>).</summary>
    public int NextInt(int min, int max) => _rng.Next(min, max);

    /// <summary>Returns a random double in [<paramref name="min"/>, <paramref name="max"/>).</summary>
    public double NextDouble(double min, double max) =>
        min + _rng.NextDouble() * (max - min);

    /// <summary>Fills <paramref name="buffer"/> with random bytes.</summary>
    public void NextBytes(byte[] buffer) => _rng.NextBytes(buffer);

    /// <summary>Shuffles a list in-place using the Fisher-Yates algorithm.</summary>
    public void Shuffle<T>(IList<T> list)
    {
        ArgumentNullException.ThrowIfNull(list);
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = _rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
