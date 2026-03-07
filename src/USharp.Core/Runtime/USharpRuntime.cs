namespace USharp.Core.Runtime;

/// <summary>
/// Manages the lifecycle of a U# runtime environment.
/// </summary>
public sealed class USharpRuntime : IDisposable
{
    private static USharpRuntime? _current;
    private readonly Dictionary<string, USharpType> _typeRegistry = new(StringComparer.Ordinal);
    private bool _disposed;

    /// <summary>Gets or sets the current ambient runtime.</summary>
    public static USharpRuntime Current
    {
        get => _current ?? throw new InvalidOperationException(
            "No U# runtime is active. Create a USharpRuntime instance first.");
        set => _current = value;
    }

    /// <summary>
    /// Registers a type with this runtime.
    /// </summary>
    public void RegisterType(USharpType type)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(type);
        _typeRegistry[type.FullName] = type;
    }

    /// <summary>
    /// Resolves a type by its fully-qualified name.
    /// </summary>
    public USharpType? ResolveType(string fullName)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        _typeRegistry.TryGetValue(fullName, out var type);
        return type;
    }

    /// <summary>Returns all registered types.</summary>
    public IEnumerable<USharpType> GetRegisteredTypes()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _typeRegistry.Values;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_current == this) _current = null;
    }
}
