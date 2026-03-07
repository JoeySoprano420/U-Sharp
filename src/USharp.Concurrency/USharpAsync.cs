namespace USharp.Concurrency;

/// <summary>
/// Provides task-based concurrency utilities for U# programs.
/// </summary>
public static class USharpAsync
{
    /// <summary>
    /// Runs <paramref name="work"/> on a thread-pool thread and returns its result.
    /// </summary>
    public static Task<T> RunAsync<T>(Func<T> work,
        CancellationToken cancellationToken = default) =>
        Task.Run(work, cancellationToken);

    /// <summary>
    /// Runs <paramref name="work"/> on a thread-pool thread.
    /// </summary>
    public static Task RunAsync(Action work,
        CancellationToken cancellationToken = default) =>
        Task.Run(work, cancellationToken);

    /// <summary>
    /// Awaits multiple tasks concurrently and returns all results.
    /// </summary>
    public static Task<T[]> WhenAll<T>(IEnumerable<Task<T>> tasks) =>
        Task.WhenAll(tasks);

    /// <summary>
    /// Awaits all tasks, ignoring individual types.
    /// </summary>
    public static Task WhenAll(IEnumerable<Task> tasks) =>
        Task.WhenAll(tasks);

    /// <summary>
    /// Returns when any task completes.
    /// </summary>
    public static Task<Task<T>> WhenAny<T>(IEnumerable<Task<T>> tasks) =>
        Task.WhenAny(tasks);

    /// <summary>
    /// Introduces a delay.
    /// </summary>
    public static Task Delay(TimeSpan delay,
        CancellationToken cancellationToken = default) =>
        Task.Delay(delay, cancellationToken);

    /// <summary>
    /// Retries <paramref name="operation"/> up to <paramref name="maxRetries"/> times
    /// with an exponential back-off.
    /// </summary>
    public static async Task<T> RetryAsync<T>(
        Func<Task<T>> operation,
        int maxRetries = 3,
        TimeSpan? initialDelay = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxRetries);
        var delay = initialDelay ?? TimeSpan.FromMilliseconds(200);
        for (int attempt = 0; ; attempt++)
        {
            try
            {
                return await operation().ConfigureAwait(false);
            }
            catch when (attempt < maxRetries - 1)
            {
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                delay = TimeSpan.FromTicks(delay.Ticks * 2);
            }
        }
    }

    /// <summary>
    /// Runs <paramref name="work"/> with a timeout; throws
    /// <see cref="TimeoutException"/> if the timeout elapses.
    /// </summary>
    public static async Task<T> WithTimeout<T>(
        Func<CancellationToken, Task<T>> work,
        TimeSpan timeout)
    {
        ArgumentNullException.ThrowIfNull(work);
        using var cts = new CancellationTokenSource(timeout);
        try
        {
            return await work(cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cts.IsCancellationRequested)
        {
            throw new TimeoutException($"Operation timed out after {timeout}.");
        }
    }
}

/// <summary>
/// A thread-safe channel allowing producer/consumer communication.
/// </summary>
/// <typeparam name="T">The message type.</typeparam>
public sealed class USharpChannel<T> : IDisposable
{
    private readonly System.Threading.Channels.Channel<T> _channel;
    private bool _disposed;

    /// <summary>Creates an unbounded channel.</summary>
    public USharpChannel()
    {
        _channel = System.Threading.Channels.Channel.CreateUnbounded<T>();
    }

    /// <summary>Creates a bounded channel with the specified capacity.</summary>
    public USharpChannel(int capacity)
    {
        _channel = System.Threading.Channels.Channel.CreateBounded<T>(capacity);
    }

    /// <summary>Writes an item to the channel.</summary>
    public ValueTask WriteAsync(T item, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _channel.Writer.WriteAsync(item, cancellationToken);
    }

    /// <summary>Reads the next item from the channel.</summary>
    public ValueTask<T> ReadAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _channel.Reader.ReadAsync(cancellationToken);
    }

    /// <summary>Tries to read an item without waiting.</summary>
    public bool TryRead(out T item)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _channel.Reader.TryRead(out item!);
    }

    /// <summary>Signals that no more items will be written.</summary>
    public void Complete() => _channel.Writer.Complete();

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _channel.Writer.TryComplete();
    }
}

/// <summary>
/// A reusable lock that supports async/await.
/// </summary>
public sealed class USharpLock : IDisposable
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private bool _disposed;

    /// <summary>Acquires the lock, waiting if necessary.</summary>
    public Task AcquireAsync(CancellationToken cancellationToken = default) =>
        _semaphore.WaitAsync(cancellationToken);

    /// <summary>Acquires the lock synchronously.</summary>
    public void Acquire() => _semaphore.Wait();

    /// <summary>Releases the lock.</summary>
    public void Release() => _semaphore.Release();

    /// <summary>
    /// Acquires the lock for the duration of <paramref name="action"/>.
    /// </summary>
    public async Task ExecuteAsync(Func<Task> action,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await action().ConfigureAwait(false);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _semaphore.Dispose();
    }
}
