namespace USharp.Core.Runtime;

/// <summary>
/// The base type for all U# objects.
/// </summary>
public abstract class USharpObject
{
    /// <summary>Gets the runtime type of this object.</summary>
    public abstract USharpType GetUSharpType();

    /// <inheritdoc />
    public override string ToString() => GetUSharpType().FullName;

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is USharpObject other && GetHashCode() == other.GetHashCode();
    }

    /// <inheritdoc />
    public override int GetHashCode() =>
        System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this);
}
