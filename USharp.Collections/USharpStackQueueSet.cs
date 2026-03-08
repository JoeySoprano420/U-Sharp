namespace USharp.Collections;

/// <summary>
/// A generic stack (LIFO) data structure for U# programs.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
public sealed class USharpStack<T> : IEnumerable<T>
{
    private readonly Stack<T> _inner = new();

    /// <summary>Gets the number of elements in the stack.</summary>
    public int Count => _inner.Count;

    /// <summary>Returns <see langword="true"/> if the stack is empty.</summary>
    public bool IsEmpty => _inner.Count == 0;

    /// <summary>Pushes an item onto the top of the stack.</summary>
    public void Push(T item) => _inner.Push(item);

    /// <summary>Removes and returns the top item.</summary>
    public T Pop() => _inner.Pop();

    /// <summary>Returns the top item without removing it.</summary>
    public T Peek() => _inner.Peek();

    /// <summary>Attempts to pop the top item.</summary>
    public bool TryPop(out T item) => _inner.TryPop(out item!);

    /// <summary>Attempts to peek at the top item.</summary>
    public bool TryPeek(out T item) => _inner.TryPeek(out item!);

    /// <summary>Removes all items from the stack.</summary>
    public void Clear() => _inner.Clear();

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => _inner.GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() =>
        GetEnumerator();

    /// <inheritdoc />
    public override string ToString() => $"USharpStack<{typeof(T).Name}>[{Count}]";
}

/// <summary>
/// A generic queue (FIFO) data structure for U# programs.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
public sealed class USharpQueue<T> : IEnumerable<T>
{
    private readonly Queue<T> _inner = new();

    /// <summary>Gets the number of elements in the queue.</summary>
    public int Count => _inner.Count;

    /// <summary>Returns <see langword="true"/> if the queue is empty.</summary>
    public bool IsEmpty => _inner.Count == 0;

    /// <summary>Enqueues an item at the back of the queue.</summary>
    public void Enqueue(T item) => _inner.Enqueue(item);

    /// <summary>Dequeues the item at the front of the queue.</summary>
    public T Dequeue() => _inner.Dequeue();

    /// <summary>Returns the front item without removing it.</summary>
    public T Peek() => _inner.Peek();

    /// <summary>Attempts to dequeue an item.</summary>
    public bool TryDequeue(out T item) => _inner.TryDequeue(out item!);

    /// <summary>Attempts to peek at the front item.</summary>
    public bool TryPeek(out T item) => _inner.TryPeek(out item!);

    /// <summary>Removes all items from the queue.</summary>
    public void Clear() => _inner.Clear();

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => _inner.GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() =>
        GetEnumerator();

    /// <inheritdoc />
    public override string ToString() => $"USharpQueue<{typeof(T).Name}>[{Count}]";
}

/// <summary>
/// A set that does not allow duplicate elements.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
public sealed class USharpSet<T> : ICollection<T>
{
    private readonly HashSet<T> _inner;

    /// <summary>Creates an empty set.</summary>
    public USharpSet() => _inner = [];

    /// <summary>Creates a set with the specified equality comparer.</summary>
    public USharpSet(IEqualityComparer<T> comparer) => _inner = new(comparer);

    /// <summary>Creates a set from an existing sequence.</summary>
    public USharpSet(IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        _inner = [.. source];
    }

    /// <inheritdoc />
    public int Count => _inner.Count;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public void Add(T item) => _inner.Add(item);

    /// <inheritdoc />
    public bool Contains(T item) => _inner.Contains(item);

    /// <inheritdoc />
    public bool Remove(T item) => _inner.Remove(item);

    /// <inheritdoc />
    public void Clear() => _inner.Clear();

    /// <summary>Returns the union of this set and <paramref name="other"/>.</summary>
    public USharpSet<T> Union(USharpSet<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);
        var result = new USharpSet<T>(this);
        result._inner.UnionWith(other._inner);
        return result;
    }

    /// <summary>Returns the intersection of this set and <paramref name="other"/>.</summary>
    public USharpSet<T> Intersect(USharpSet<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);
        var result = new USharpSet<T>(this);
        result._inner.IntersectWith(other._inner);
        return result;
    }

    /// <summary>Returns the difference of this set minus <paramref name="other"/>.</summary>
    public USharpSet<T> Except(USharpSet<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);
        var result = new USharpSet<T>(this);
        result._inner.ExceptWith(other._inner);
        return result;
    }

    /// <summary>Returns <see langword="true"/> if this set is a subset of <paramref name="other"/>.</summary>
    public bool IsSubsetOf(USharpSet<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);
        return _inner.IsSubsetOf(other._inner);
    }

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex) => _inner.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => _inner.GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() =>
        GetEnumerator();

    /// <inheritdoc />
    public override string ToString() => $"USharpSet<{typeof(T).Name}>[{Count}]";
}
