namespace USharp.Math;

/// <summary>
/// Provides common mathematical functions for U# programs.
/// </summary>
public static class USharpMath
{
    /// <summary>Mathematical constant π.</summary>
    public const double Pi = System.Math.PI;

    /// <summary>Mathematical constant e (Euler's number).</summary>
    public const double E = System.Math.E;

    /// <summary>Returns the absolute value of <paramref name="value"/>.</summary>
    public static double Abs(double value) => System.Math.Abs(value);

    /// <summary>Returns the absolute value of <paramref name="value"/>.</summary>
    public static int Abs(int value) => System.Math.Abs(value);

    /// <summary>Returns the ceiling of <paramref name="value"/>.</summary>
    public static double Ceiling(double value) => System.Math.Ceiling(value);

    /// <summary>Returns the floor of <paramref name="value"/>.</summary>
    public static double Floor(double value) => System.Math.Floor(value);

    /// <summary>Rounds <paramref name="value"/> to the nearest integer.</summary>
    public static double Round(double value) => System.Math.Round(value);

    /// <summary>Rounds <paramref name="value"/> to <paramref name="digits"/> decimal places.</summary>
    public static double Round(double value, int digits) =>
        System.Math.Round(value, digits);

    /// <summary>Returns the square root of <paramref name="value"/>.</summary>
    public static double Sqrt(double value) => System.Math.Sqrt(value);

    /// <summary>Returns <paramref name="base"/> raised to the power of <paramref name="exponent"/>.</summary>
    public static double Pow(double @base, double exponent) =>
        System.Math.Pow(@base, exponent);

    /// <summary>Returns the natural logarithm of <paramref name="value"/>.</summary>
    public static double Log(double value) => System.Math.Log(value);

    /// <summary>Returns the base-10 logarithm of <paramref name="value"/>.</summary>
    public static double Log10(double value) => System.Math.Log10(value);

    /// <summary>Returns the logarithm of <paramref name="value"/> in the specified base.</summary>
    public static double Log(double value, double newBase) =>
        System.Math.Log(value, newBase);

    /// <summary>Returns the sine of <paramref name="angle"/> (in radians).</summary>
    public static double Sin(double angle) => System.Math.Sin(angle);

    /// <summary>Returns the cosine of <paramref name="angle"/> (in radians).</summary>
    public static double Cos(double angle) => System.Math.Cos(angle);

    /// <summary>Returns the tangent of <paramref name="angle"/> (in radians).</summary>
    public static double Tan(double angle) => System.Math.Tan(angle);

    /// <summary>Returns the arc-sine of <paramref name="value"/>.</summary>
    public static double Asin(double value) => System.Math.Asin(value);

    /// <summary>Returns the arc-cosine of <paramref name="value"/>.</summary>
    public static double Acos(double value) => System.Math.Acos(value);

    /// <summary>Returns the arc-tangent of <paramref name="value"/>.</summary>
    public static double Atan(double value) => System.Math.Atan(value);

    /// <summary>Returns the angle whose tangent is the quotient of two specified numbers.</summary>
    public static double Atan2(double y, double x) => System.Math.Atan2(y, x);

    /// <summary>Returns the larger of two values.</summary>
    public static T Max<T>(T a, T b) where T : IComparable<T> =>
        a.CompareTo(b) >= 0 ? a : b;

    /// <summary>Returns the smaller of two values.</summary>
    public static T Min<T>(T a, T b) where T : IComparable<T> =>
        a.CompareTo(b) <= 0 ? a : b;

    /// <summary>Clamps <paramref name="value"/> between <paramref name="min"/> and <paramref name="max"/>.</summary>
    public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0) return min;
        if (value.CompareTo(max) > 0) return max;
        return value;
    }

    /// <summary>
    /// Returns the greatest common divisor of two non-negative integers.
    /// </summary>
    public static long Gcd(long a, long b)
    {
        a = System.Math.Abs(a);
        b = System.Math.Abs(b);
        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    /// <summary>
    /// Returns the least common multiple of two non-negative integers.
    /// </summary>
    public static long Lcm(long a, long b) =>
        a == 0 || b == 0 ? 0 : System.Math.Abs(a / Gcd(a, b) * b);

    /// <summary>Returns <see langword="true"/> if <paramref name="value"/> is a prime number.</summary>
    public static bool IsPrime(long value)
    {
        if (value < 2) return false;
        if (value == 2) return true;
        if (value % 2 == 0) return false;
        for (long i = 3; i * i <= value; i += 2)
            if (value % i == 0) return false;
        return true;
    }

    /// <summary>Converts degrees to radians.</summary>
    public static double ToRadians(double degrees) => degrees * Pi / 180.0;

    /// <summary>Converts radians to degrees.</summary>
    public static double ToDegrees(double radians) => radians * 180.0 / Pi;

    /// <summary>Calculates the factorial of a non-negative integer.</summary>
    public static long Factorial(int n)
    {
        if (n < 0) throw new ArgumentOutOfRangeException(nameof(n),
            "Factorial is not defined for negative numbers.");
        long result = 1;
        for (int i = 2; i <= n; i++) result *= i;
        return result;
    }

    /// <summary>
    /// Performs linear interpolation between <paramref name="a"/> and <paramref name="b"/>
    /// at parameter <paramref name="t"/> (0 to 1).
    /// </summary>
    public static double Lerp(double a, double b, double t) =>
        a + (b - a) * t;
}
