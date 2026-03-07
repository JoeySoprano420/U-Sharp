using System.Diagnostics;

namespace USharp.Diagnostics;

/// <summary>
/// Provides performance profiling utilities for U# programs.
/// </summary>
public sealed class USharpProfiler
{
    private readonly Dictionary<string, List<TimeSpan>> _measurements =
        new(StringComparer.Ordinal);

    /// <summary>
    /// Measures the elapsed time of <paramref name="action"/> and records it
    /// under <paramref name="label"/>.
    /// </summary>
    public TimeSpan Measure(string label, Action action)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        ArgumentNullException.ThrowIfNull(action);
        var sw = Stopwatch.StartNew();
        action();
        sw.Stop();
        Record(label, sw.Elapsed);
        return sw.Elapsed;
    }

    /// <summary>
    /// Measures the elapsed time of <paramref name="func"/> asynchronously and
    /// records it under <paramref name="label"/>.
    /// </summary>
    public async Task<TimeSpan> MeasureAsync(string label, Func<Task> func)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        ArgumentNullException.ThrowIfNull(func);
        var sw = Stopwatch.StartNew();
        await func().ConfigureAwait(false);
        sw.Stop();
        Record(label, sw.Elapsed);
        return sw.Elapsed;
    }

    private void Record(string label, TimeSpan elapsed)
    {
        if (!_measurements.TryGetValue(label, out var list))
        {
            list = [];
            _measurements[label] = list;
        }
        list.Add(elapsed);
    }

    /// <summary>Returns a summary of all recorded measurements.</summary>
    public IReadOnlyDictionary<string, ProfilerStats> GetStats()
    {
        var result = new Dictionary<string, ProfilerStats>(_measurements.Count);
        foreach (var (label, times) in _measurements)
        {
            if (times.Count == 0) continue;
            var ticks = times.Select(t => t.Ticks).ToList();
            result[label] = new ProfilerStats(
                Count: times.Count,
                Total: TimeSpan.FromTicks(ticks.Sum()),
                Min: TimeSpan.FromTicks(ticks.Min()),
                Max: TimeSpan.FromTicks(ticks.Max()),
                Average: TimeSpan.FromTicks((long)ticks.Average()));
        }
        return result;
    }

    /// <summary>Resets all recorded measurements.</summary>
    public void Reset() => _measurements.Clear();
}

/// <summary>
/// Statistics for a profiler label.
/// </summary>
public sealed record ProfilerStats(
    int Count,
    TimeSpan Total,
    TimeSpan Min,
    TimeSpan Max,
    TimeSpan Average)
{
    /// <inheritdoc />
    public override string ToString() =>
        $"Count={Count} | Total={Total.TotalMilliseconds:F2}ms | " +
        $"Avg={Average.TotalMilliseconds:F2}ms | " +
        $"Min={Min.TotalMilliseconds:F2}ms | Max={Max.TotalMilliseconds:F2}ms";
}

/// <summary>
/// A scoped timer that records its measurement on disposal.
/// </summary>
public sealed class USharpTimer : IDisposable
{
    private readonly string _label;
    private readonly USharpProfiler _profiler;
    private readonly Stopwatch _sw = Stopwatch.StartNew();
    private bool _disposed;

    /// <summary>Starts a new scoped timer.</summary>
    public USharpTimer(string label, USharpProfiler profiler)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        ArgumentNullException.ThrowIfNull(profiler);
        _label = label;
        _profiler = profiler;
    }

    /// <summary>Gets the elapsed time so far.</summary>
    public TimeSpan Elapsed => _sw.Elapsed;

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _sw.Stop();
        _profiler.Measure(_label, () => { }); // Record via public API
    }
}
