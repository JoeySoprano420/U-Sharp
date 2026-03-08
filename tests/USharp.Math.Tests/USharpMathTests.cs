using USharp.Math;
using Xunit;

namespace USharp.Math.Tests;

public sealed class USharpMathTests
{
    [Theory]
    [InlineData(9.0, 3.0)]
    [InlineData(4.0, 2.0)]
    [InlineData(0.0, 0.0)]
    public void Sqrt_ReturnsCorrectValue(double input, double expected)
    {
        Assert.Equal(expected, USharpMath.Sqrt(input), precision: 10);
    }

    [Theory]
    [InlineData(2.0, 10.0, 1024.0)]
    [InlineData(3.0, 3.0, 27.0)]
    public void Pow_ReturnsCorrectValue(double @base, double exp, double expected)
    {
        Assert.Equal(expected, USharpMath.Pow(@base, exp), precision: 10);
    }

    [Fact]
    public void Abs_NegativeValue_ReturnsPositive()
    {
        Assert.Equal(5.0, USharpMath.Abs(-5.0));
    }

    [Fact]
    public void Clamp_BelowMin_ReturnsMin()
    {
        Assert.Equal(0, USharpMath.Clamp(-5, 0, 10));
    }

    [Fact]
    public void Clamp_AboveMax_ReturnsMax()
    {
        Assert.Equal(10, USharpMath.Clamp(15, 0, 10));
    }

    [Fact]
    public void Clamp_InRange_ReturnsSameValue()
    {
        Assert.Equal(5, USharpMath.Clamp(5, 0, 10));
    }

    [Theory]
    [InlineData(12, 8, 4)]
    [InlineData(7, 3, 1)]
    [InlineData(0, 5, 5)]
    public void Gcd_ReturnsCorrectValue(long a, long b, long expected)
    {
        Assert.Equal(expected, USharpMath.Gcd(a, b));
    }

    [Theory]
    [InlineData(4, 6, 12)]
    [InlineData(3, 7, 21)]
    public void Lcm_ReturnsCorrectValue(long a, long b, long expected)
    {
        Assert.Equal(expected, USharpMath.Lcm(a, b));
    }

    [Theory]
    [InlineData(2, true)]
    [InlineData(3, true)]
    [InlineData(17, true)]
    [InlineData(4, false)]
    [InlineData(1, false)]
    [InlineData(0, false)]
    public void IsPrime_ReturnsCorrectResult(long value, bool expected)
    {
        Assert.Equal(expected, USharpMath.IsPrime(value));
    }

    [Fact]
    public void Factorial_Zero_ReturnsOne()
    {
        Assert.Equal(1L, USharpMath.Factorial(0));
    }

    [Fact]
    public void Factorial_Five_Returns120()
    {
        Assert.Equal(120L, USharpMath.Factorial(5));
    }

    [Fact]
    public void Factorial_Negative_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => USharpMath.Factorial(-1));
    }

    [Fact]
    public void Lerp_HalfWay_ReturnsMidpoint()
    {
        Assert.Equal(5.0, USharpMath.Lerp(0.0, 10.0, 0.5));
    }

    [Fact]
    public void ToRadians_180Degrees_ReturnsPi()
    {
        Assert.Equal(USharpMath.Pi, USharpMath.ToRadians(180.0), precision: 10);
    }
}

public sealed class USharpStatsTests
{
    [Fact]
    public void Mean_ReturnsCorrectValue()
    {
        Assert.Equal(3.0, USharpStats.Mean([1.0, 2.0, 3.0, 4.0, 5.0]));
    }

    [Fact]
    public void Median_OddCount_ReturnsMiddleValue()
    {
        Assert.Equal(3.0, USharpStats.Median([5.0, 1.0, 3.0]));
    }

    [Fact]
    public void Median_EvenCount_ReturnsAverageOfMiddleTwo()
    {
        Assert.Equal(2.5, USharpStats.Median([1.0, 2.0, 3.0, 4.0]));
    }

    [Fact]
    public void Sum_ReturnsCorrectValue()
    {
        Assert.Equal(15.0, USharpStats.Sum([1.0, 2.0, 3.0, 4.0, 5.0]));
    }

    [Fact]
    public void Range_ReturnsDifference()
    {
        Assert.Equal(9.0, USharpStats.Range([1.0, 4.0, 7.0, 10.0]));
    }

    [Fact]
    public void StdDev_KnownValues_IsCorrect()
    {
        // Mean=3, variance=2, stddev≈1.414
        var stdDev = USharpStats.StdDev([1.0, 2.0, 3.0, 4.0, 5.0]);
        Assert.Equal(System.Math.Sqrt(2), stdDev, precision: 10);
    }
}

public sealed class USharpRandomTests
{
    [Fact]
    public void NextDouble_InRange()
    {
        var rng = new USharpRandom(42);
        for (int i = 0; i < 100; i++)
        {
            var v = rng.NextDouble();
            Assert.InRange(v, 0.0, 1.0);
        }
    }

    [Fact]
    public void NextInt_InRange()
    {
        var rng = new USharpRandom(0);
        for (int i = 0; i < 50; i++)
        {
            var v = rng.NextInt(0, 10);
            Assert.InRange(v, 0, 9);
        }
    }

    [Fact]
    public void Shuffle_ChangesOrder()
    {
        var rng = new USharpRandom(123);
        var list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var original = list.ToList();
        rng.Shuffle(list);
        // Not equal in some order (very likely with seeded rng)
        Assert.NotEqual(original, list);
    }
}
