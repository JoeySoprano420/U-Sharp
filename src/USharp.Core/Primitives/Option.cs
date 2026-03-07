namespace USharp.Core.Primitives;

/// <summary>
/// Represents the U# optional type (a value that may or may not be present).
/// </summary>
/// <typeparam name="T">The type of the contained value.</typeparam>
public readonly struct Option<T> : IEquatable<Option<T>>
{
    private readonly T? _value;

    private Option(T value)
    {
        _value = value;
        HasValue = true;
    }

    /// <summary>Gets whether this option contains a value.</summary>
    public bool HasValue { get; }

    /// <summary>Gets the contained value, or throws if none.</summary>
    public T Value => HasValue
        ? _value!
        : throw new InvalidOperationException("Option has no value.");

    /// <summary>Creates an option with a value.</summary>
    public static Option<T> Some(T value) => new(value);

    /// <summary>Gets the empty option.</summary>
    public static Option<T> None => default;

    /// <summary>Returns the value or a default.</summary>
    public T GetValueOrDefault(T defaultValue = default!) =>
        HasValue ? _value! : defaultValue;

    /// <summary>Maps this option using a transform function.</summary>
    public Option<TResult> Map<TResult>(Func<T, TResult> map)
    {
        ArgumentNullException.ThrowIfNull(map);
        return HasValue ? Option<TResult>.Some(map(_value!)) : Option<TResult>.None;
    }

    /// <summary>Binds this option to another optional-returning function.</summary>
    public Option<TResult> Bind<TResult>(Func<T, Option<TResult>> bind)
    {
        ArgumentNullException.ThrowIfNull(bind);
        return HasValue ? bind(_value!) : Option<TResult>.None;
    }

    /// <inheritdoc />
    public bool Equals(Option<T> other)
    {
        if (!HasValue && !other.HasValue) return true;
        if (HasValue != other.HasValue) return false;
        return EqualityComparer<T>.Default.Equals(_value, other._value);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Option<T> other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HasValue ? HashCode.Combine(true, _value) : 0;

    /// <inheritdoc />
    public override string ToString() => HasValue ? $"Some({_value})" : "None";

    /// <summary>Equality operator.</summary>
    public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);

    /// <summary>Inequality operator.</summary>
    public static bool operator !=(Option<T> left, Option<T> right) => !left.Equals(right);
}

/// <summary>
/// Represents the U# result type: either a successful value or an error.
/// </summary>
/// <typeparam name="TValue">The success value type.</typeparam>
/// <typeparam name="TError">The error type.</typeparam>
public readonly struct Result<TValue, TError> : IEquatable<Result<TValue, TError>>
{
    private readonly TValue? _value;
    private readonly TError? _error;

    private Result(TValue value)
    {
        _value = value;
        IsSuccess = true;
    }

    private Result(TError error, bool _)
    {
        _error = error;
        IsSuccess = false;
    }

    /// <summary>Gets whether this result represents success.</summary>
    public bool IsSuccess { get; }

    /// <summary>Gets whether this result represents an error.</summary>
    public bool IsError => !IsSuccess;

    /// <summary>Gets the success value, or throws if this is an error result.</summary>
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Result is in error state.");

    /// <summary>Gets the error value, or throws if this is a success result.</summary>
    public TError Error => !IsSuccess
        ? _error!
        : throw new InvalidOperationException("Result is in success state.");

    /// <summary>Creates a success result.</summary>
    public static Result<TValue, TError> Ok(TValue value) => new(value);

    /// <summary>Creates an error result.</summary>
    public static Result<TValue, TError> Err(TError error) => new(error, false);

    /// <summary>Maps the success value using a transform.</summary>
    public Result<TResult, TError> Map<TResult>(Func<TValue, TResult> map)
    {
        ArgumentNullException.ThrowIfNull(map);
        return IsSuccess
            ? Result<TResult, TError>.Ok(map(_value!))
            : Result<TResult, TError>.Err(_error!);
    }

    /// <inheritdoc />
    public bool Equals(Result<TValue, TError> other)
    {
        if (IsSuccess != other.IsSuccess) return false;
        if (IsSuccess) return EqualityComparer<TValue>.Default.Equals(_value, other._value);
        return EqualityComparer<TError>.Default.Equals(_error, other._error);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) =>
        obj is Result<TValue, TError> other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => IsSuccess
        ? HashCode.Combine(true, _value)
        : HashCode.Combine(false, _error);

    /// <inheritdoc />
    public override string ToString() => IsSuccess ? $"Ok({_value})" : $"Err({_error})";

    /// <summary>Equality operator.</summary>
    public static bool operator ==(Result<TValue, TError> left, Result<TValue, TError> right) =>
        left.Equals(right);

    /// <summary>Inequality operator.</summary>
    public static bool operator !=(Result<TValue, TError> left, Result<TValue, TError> right) =>
        !left.Equals(right);
}
