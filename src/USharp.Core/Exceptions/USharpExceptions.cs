namespace USharp.Core.Exceptions;

/// <summary>
/// Base class for all U# language exceptions.
/// </summary>
public class USharpException : Exception
{
    /// <inheritdoc cref="Exception()"/>
    public USharpException() { }

    /// <inheritdoc cref="Exception(string)"/>
    public USharpException(string message) : base(message) { }

    /// <inheritdoc cref="Exception(string, Exception)"/>
    public USharpException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>
/// Thrown when a runtime type error occurs in U#.
/// </summary>
public sealed class USharpTypeException : USharpException
{
    /// <inheritdoc cref="USharpException(string)"/>
    public USharpTypeException(string message) : base(message) { }

    /// <inheritdoc cref="USharpException(string, Exception)"/>
    public USharpTypeException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>
/// Thrown when a null-reference is encountered where a value is required.
/// </summary>
public sealed class USharpNullException : USharpException
{
    /// <inheritdoc cref="USharpException(string)"/>
    public USharpNullException(string message) : base(message) { }
}

/// <summary>
/// Thrown when an index is out of range.
/// </summary>
public sealed class USharpIndexOutOfRangeException : USharpException
{
    /// <summary>The invalid index that caused the exception.</summary>
    public int Index { get; }

    /// <summary>Creates a new <see cref="USharpIndexOutOfRangeException"/>.</summary>
    public USharpIndexOutOfRangeException(int index)
        : base($"Index {index} was out of range.")
    {
        Index = index;
    }
}

/// <summary>
/// Thrown when a method or feature is not yet implemented.
/// </summary>
public sealed class USharpNotImplementedException : USharpException
{
    /// <inheritdoc cref="USharpException(string)"/>
    public USharpNotImplementedException(string message) : base(message) { }

    /// <summary>
    /// Creates an exception indicating that <paramref name="memberName"/> is not implemented.
    /// </summary>
    public static USharpNotImplementedException For(string memberName) =>
        new($"'{memberName}' is not yet implemented in U#.");
}

/// <summary>
/// Thrown when a runtime operation fails due to an invalid argument.
/// </summary>
public sealed class USharpArgumentException : USharpException
{
    /// <summary>The name of the offending parameter.</summary>
    public string? ParamName { get; }

    /// <summary>Creates a new <see cref="USharpArgumentException"/>.</summary>
    public USharpArgumentException(string message, string? paramName = null)
        : base(message)
    {
        ParamName = paramName;
    }
}
