namespace USharp.Core.Runtime;

/// <summary>
/// Represents a type in the U# type system.
/// </summary>
public sealed class USharpType
{
    private readonly string _name;
    private readonly string _namespace;
    private readonly USharpType? _baseType;
    private readonly IReadOnlyList<USharpType> _interfaces;

    /// <summary>Creates a new <see cref="USharpType"/>.</summary>
    public USharpType(
        string name,
        string @namespace,
        USharpType? baseType = null,
        IReadOnlyList<USharpType>? interfaces = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(@namespace);
        _name = name;
        _namespace = @namespace;
        _baseType = baseType;
        _interfaces = interfaces ?? [];
    }

    /// <summary>Gets the simple name of this type.</summary>
    public string Name => _name;

    /// <summary>Gets the namespace of this type.</summary>
    public string Namespace => _namespace;

    /// <summary>Gets the fully-qualified name (<c>Namespace.Name</c>).</summary>
    public string FullName => string.IsNullOrEmpty(_namespace) ? _name : $"{_namespace}.{_name}";

    /// <summary>Gets the base type, or <see langword="null"/> for root types.</summary>
    public USharpType? BaseType => _baseType;

    /// <summary>Gets the interfaces implemented by this type.</summary>
    public IReadOnlyList<USharpType> Interfaces => _interfaces;

    /// <summary>
    /// Returns <see langword="true"/> if this type is assignable from
    /// <paramref name="other"/> (i.e. <paramref name="other"/> is this type
    /// or a derived type).
    /// </summary>
    public bool IsAssignableFrom(USharpType other)
    {
        ArgumentNullException.ThrowIfNull(other);
        USharpType? current = other;
        while (current is not null)
        {
            if (current == this) return true;
            current = current.BaseType;
        }
        return false;
    }

    /// <inheritdoc />
    public override string ToString() => FullName;

    /// <inheritdoc />
    public override bool Equals(object? obj) =>
        obj is USharpType other && FullName == other.FullName;

    /// <inheritdoc />
    public override int GetHashCode() => FullName.GetHashCode();
}
