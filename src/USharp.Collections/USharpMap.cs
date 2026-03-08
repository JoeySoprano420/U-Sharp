namespace USharp.Collections;

/// <summary>
/// A key/value map for U# programs with a fluent API.
/// </summary>
/// <typeparam name="TKey">The key type.</typeparam>
/// <typeparam name="TValue">The value type.</typeparam>
public sealed class USharpMap<TKey, TValue> : IDictionary<TKey, TValue>
    where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _inner;

    /// <summary>Creates an empty map.</summary>
    public USharpMap() => _inner = [];

    /// <summary>Creates a map with the specified initial capacity.</summary>
    public USharpMap(int capacity) => _inner = new Dictionary<TKey, TValue>(capacity);

    /// <summary>Creates a map with the specified equality comparer.</summary>
    public USharpMap(IEqualityComparer<TKey> comparer) => _inner = new(comparer);

    /// <summary>Creates a map from an existing dictionary.</summary>
    public USharpMap(IDictionary<TKey, TValue> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        _inner = new Dictionary<TKey, TValue>(source);
    }

    /// <inheritdoc />
    public TValue this[TKey key]
    {
        get => _inner[key];
        set => _inner[key] = value;
    }

    /// <inheritdoc />
    public ICollection<TKey> Keys => _inner.Keys;

    /// <inheritdoc />
    public ICollection<TValue> Values => _inner.Values;

    /// <inheritdoc />
    public int Count => _inner.Count;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public void Add(TKey key, TValue value) => _inner.Add(key, value);

    /// <inheritdoc />
    public void Add(KeyValuePair<TKey, TValue> item) =>
        ((IDictionary<TKey, TValue>)_inner).Add(item);

    /// <inheritdoc />
    public void Clear() => _inner.Clear();

    /// <inheritdoc />
    public bool Contains(KeyValuePair<TKey, TValue> item) =>
        ((IDictionary<TKey, TValue>)_inner).Contains(item);

    /// <inheritdoc />
    public bool ContainsKey(TKey key) => _inner.ContainsKey(key);

    /// <inheritdoc />
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
        ((IDictionary<TKey, TValue>)_inner).CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _inner.GetEnumerator();

    /// <inheritdoc />
    public bool Remove(TKey key) => _inner.Remove(key);

    /// <inheritdoc />
    public bool Remove(KeyValuePair<TKey, TValue> item) =>
        ((IDictionary<TKey, TValue>)_inner).Remove(item);

    /// <inheritdoc />
    public bool TryGetValue(TKey key, out TValue value) =>
        _inner.TryGetValue(key, out value!);

    /// <summary>
    /// Returns the value for <paramref name="key"/>, or <paramref name="defaultValue"/>
    /// if the key is not found.
    /// </summary>
    public TValue GetOrDefault(TKey key, TValue defaultValue) =>
        _inner.TryGetValue(key, out var val) ? val : defaultValue;

    /// <summary>
    /// Gets or adds a value for <paramref name="key"/> using <paramref name="factory"/>.
    /// </summary>
    public TValue GetOrAdd(TKey key, Func<TKey, TValue> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        if (_inner.TryGetValue(key, out var existing)) return existing;
        var value = factory(key);
        _inner[key] = value;
        return value;
    }

    /// <summary>
    /// Returns a new map containing only entries where the value satisfies
    /// <paramref name="predicate"/>.
    /// </summary>
    public USharpMap<TKey, TValue> Filter(Func<TKey, TValue, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        var result = new USharpMap<TKey, TValue>();
        foreach (var (k, v) in _inner)
            if (predicate(k, v)) result.Add(k, v);
        return result;
    }

    /// <summary>Merges another map into this one. Existing keys are overwritten.</summary>
    public void Merge(USharpMap<TKey, TValue> other)
    {
        ArgumentNullException.ThrowIfNull(other);
        foreach (var (k, v) in other._inner)
            _inner[k] = v;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() =>
        GetEnumerator();

    /// <inheritdoc />
    public override string ToString() =>
        $"USharpMap<{typeof(TKey).Name},{typeof(TValue).Name}>[{Count}]";
}
