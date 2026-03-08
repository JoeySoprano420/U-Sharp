namespace USharp.Diagnostics;

/// <summary>
/// Log severity levels for U# programs.
/// </summary>
public enum USharpLogLevel
{
    /// <summary>Detailed diagnostic information.</summary>
    Debug = 0,
    /// <summary>Informational messages.</summary>
    Info = 1,
    /// <summary>Potentially harmful situations.</summary>
    Warning = 2,
    /// <summary>Error events that might still allow the program to continue.</summary>
    Error = 3,
    /// <summary>Severe error events that lead to application abort.</summary>
    Fatal = 4,
}

/// <summary>
/// Represents a single log entry.
/// </summary>
public sealed record USharpLogEntry(
    DateTimeOffset Timestamp,
    USharpLogLevel Level,
    string Message,
    Exception? Exception = null)
{
    /// <inheritdoc />
    public override string ToString()
    {
        var exPart = Exception is null ? "" : $" [{Exception.GetType().Name}: {Exception.Message}]";
        return $"[{Timestamp:HH:mm:ss.fff}] [{Level,-7}] {Message}{exPart}";
    }
}

/// <summary>
/// A structured logger for U# programs.
/// </summary>
public sealed class USharpLogger
{
    private readonly string _name;
    private USharpLogLevel _minimumLevel;
    private readonly List<Action<USharpLogEntry>> _sinks = [];

    /// <summary>Creates a logger with the specified name.</summary>
    public USharpLogger(string name, USharpLogLevel minimumLevel = USharpLogLevel.Debug)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _name = name;
        _minimumLevel = minimumLevel;

        // Default sink: write to console
        _sinks.Add(entry => System.Console.WriteLine(entry));
    }

    /// <summary>Gets the logger name.</summary>
    public string Name => _name;

    /// <summary>Sets the minimum log level to emit.</summary>
    public USharpLogger WithMinimumLevel(USharpLogLevel level)
    {
        _minimumLevel = level;
        return this;
    }

    /// <summary>Adds a custom log sink.</summary>
    public USharpLogger AddSink(Action<USharpLogEntry> sink)
    {
        ArgumentNullException.ThrowIfNull(sink);
        _sinks.Add(sink);
        return this;
    }

    /// <summary>Logs a debug message.</summary>
    public void Debug(string message, Exception? exception = null) =>
        Log(USharpLogLevel.Debug, message, exception);

    /// <summary>Logs an informational message.</summary>
    public void Info(string message, Exception? exception = null) =>
        Log(USharpLogLevel.Info, message, exception);

    /// <summary>Logs a warning.</summary>
    public void Warning(string message, Exception? exception = null) =>
        Log(USharpLogLevel.Warning, message, exception);

    /// <summary>Logs an error.</summary>
    public void Error(string message, Exception? exception = null) =>
        Log(USharpLogLevel.Error, message, exception);

    /// <summary>Logs a fatal error.</summary>
    public void Fatal(string message, Exception? exception = null) =>
        Log(USharpLogLevel.Fatal, message, exception);

    private void Log(USharpLogLevel level, string message, Exception? exception)
    {
        if (level < _minimumLevel) return;
        var entry = new USharpLogEntry(DateTimeOffset.UtcNow, level, message, exception);
        foreach (var sink in _sinks)
            sink(entry);
    }
}

/// <summary>
/// A factory for creating named loggers.
/// </summary>
public sealed class USharpLoggerFactory
{
    private readonly Dictionary<string, USharpLogger> _loggers =
        new(StringComparer.Ordinal);
    private USharpLogLevel _defaultLevel = USharpLogLevel.Debug;

    /// <summary>Sets the default minimum log level for new loggers.</summary>
    public USharpLoggerFactory WithDefaultLevel(USharpLogLevel level)
    {
        _defaultLevel = level;
        return this;
    }

    /// <summary>Gets or creates a logger with the given name.</summary>
    public USharpLogger GetLogger(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (!_loggers.TryGetValue(name, out var logger))
        {
            logger = new USharpLogger(name, _defaultLevel);
            _loggers[name] = logger;
        }
        return logger;
    }

    /// <summary>Gets a logger named after the type <typeparamref name="T"/>.</summary>
    public USharpLogger GetLogger<T>() => GetLogger(typeof(T).FullName ?? typeof(T).Name);
}
