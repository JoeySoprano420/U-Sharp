namespace USharp.Core.Primitives;

/// <summary>
/// Represents an immutable range of values with an inclusive start and exclusive end.
/// </summary>
public readonly struct USharpRange : IEquatable<USharpRange>
{
    /// <summary>Creates a new range.</summary>
    public USharpRange(long start, long end)
    {
        if (end < start)
            throw new ArgumentOutOfRangeException(nameof(end),
                "End must be greater than or equal to start.");
        Start = start;
        End = end;
    }

    /// <summary>Gets the inclusive start of the range.</summary>
    public long Start { get; }

    /// <summary>Gets the exclusive end of the range.</summary>
    public long End { get; }

    /// <summary>Gets the number of elements in the range.</summary>
    public long Count => End - Start;

    /// <summary>Returns whether <paramref name="value"/> is inside this range.</summary>
    public bool Contains(long value) => value >= Start && value < End;

    /// <summary>Enumerates all values in the range.</summary>
    public IEnumerable<long> Enumerate()
    {
        for (long i = Start; i < End; i++)
            yield return i;
    }

    /// <inheritdoc />
    public bool Equals(USharpRange other) => Start == other.Start && End == other.End;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is USharpRange other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Start, End);

    /// <inheritdoc />
    public override string ToString() => $"{Start}..{End}";

    /// <summary>Equality operator.</summary>
    public static bool operator ==(USharpRange left, USharpRange right) => left.Equals(right);

    /// <summary>Inequality operator.</summary>
    public static bool operator !=(USharpRange left, USharpRange right) => !left.Equals(right);
}
