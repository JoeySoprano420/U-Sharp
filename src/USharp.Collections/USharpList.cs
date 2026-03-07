namespace USharp.Collections;

/// <summary>
/// A strongly-typed, resizable list used by the U# runtime.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
public sealed class USharpList<T> : IList<T>, IReadOnlyList<T>
{
    private readonly List<T> _inner;

    /// <summary>Creates an empty list.</summary>
    public USharpList() => _inner = [];

    /// <summary>Creates a list with the specified initial capacity.</summary>
    public USharpList(int capacity) => _inner = new List<T>(capacity);

    /// <summary>Creates a list from an existing sequence.</summary>
    public USharpList(IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        _inner = [.. source];
    }

    /// <inheritdoc />
    public T this[int index]
    {
        get => _inner[index];
        set => _inner[index] = value;
    }

    /// <inheritdoc />
    public int Count => _inner.Count;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public void Add(T item) => _inner.Add(item);

    /// <summary>Adds a range of items.</summary>
    public void AddRange(IEnumerable<T> items) => _inner.AddRange(items);

    /// <inheritdoc />
    public void Clear() => _inner.Clear();

    /// <inheritdoc />
    public bool Contains(T item) => _inner.Contains(item);

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex) => _inner.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => _inner.GetEnumerator();

    /// <inheritdoc />
    public int IndexOf(T item) => _inner.IndexOf(item);

    /// <inheritdoc />
    public void Insert(int index, T item) => _inner.Insert(index, item);

    /// <inheritdoc />
    public bool Remove(T item) => _inner.Remove(item);

    /// <inheritdoc />
    public void RemoveAt(int index) => _inner.RemoveAt(index);

    /// <summary>
    /// Returns a new list with only the elements matching <paramref name="predicate"/>.
    /// </summary>
    public USharpList<T> Filter(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return new USharpList<T>(_inner.Where(predicate));
    }

    /// <summary>
    /// Returns a new list by applying <paramref name="transform"/> to each element.
    /// </summary>
    public USharpList<TResult> Map<TResult>(Func<T, TResult> transform)
    {
        ArgumentNullException.ThrowIfNull(transform);
        return new USharpList<TResult>(_inner.Select(transform));
    }

    /// <summary>Reduces the list to a single value.</summary>
    public TAccumulate Reduce<TAccumulate>(
        TAccumulate seed,
        Func<TAccumulate, T, TAccumulate> accumulator)
    {
        ArgumentNullException.ThrowIfNull(accumulator);
        return _inner.Aggregate(seed, accumulator);
    }

    /// <summary>Sorts the list in-place using the default comparer.</summary>
    public void Sort() => _inner.Sort();

    /// <summary>Sorts the list in-place using the specified comparison.</summary>
    public void Sort(Comparison<T> comparison) => _inner.Sort(comparison);

    /// <summary>Returns a shallow copy of a portion of the list.</summary>
    public USharpList<T> Slice(int start, int count) =>
        new(_inner.Skip(start).Take(count));

    /// <summary>Returns the first element, or <see langword="default"/> if empty.</summary>
    public T? FirstOrDefault() => _inner.Count > 0 ? _inner[0] : default;

    /// <summary>Returns the last element, or <see langword="default"/> if empty.</summary>
    public T? LastOrDefault() => _inner.Count > 0 ? _inner[^1] : default;

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() =>
        GetEnumerator();

    /// <inheritdoc />
    public override string ToString() => $"USharpList<{typeof(T).Name}>[{Count}]";
}
