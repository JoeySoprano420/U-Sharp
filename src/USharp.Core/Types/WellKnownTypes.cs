namespace USharp.Core.Types;

/// <summary>
/// Provides well-known U# type descriptors used by the standard libraries.
/// </summary>
public static class WellKnownTypes
{
    private static readonly USharp.Core.Runtime.USharpType _int =
        new("Int", "USharp.Core");
    private static readonly USharp.Core.Runtime.USharpType _long =
        new("Long", "USharp.Core");
    private static readonly USharp.Core.Runtime.USharpType _float =
        new("Float", "USharp.Core");
    private static readonly USharp.Core.Runtime.USharpType _double =
        new("Double", "USharp.Core");
    private static readonly USharp.Core.Runtime.USharpType _bool =
        new("Bool", "USharp.Core");
    private static readonly USharp.Core.Runtime.USharpType _string =
        new("String", "USharp.Core");
    private static readonly USharp.Core.Runtime.USharpType _void =
        new("Void", "USharp.Core");
    private static readonly USharp.Core.Runtime.USharpType _any =
        new("Any", "USharp.Core");

    /// <summary>The U# <c>int</c> type (32-bit signed integer).</summary>
    public static USharp.Core.Runtime.USharpType Int => _int;

    /// <summary>The U# <c>long</c> type (64-bit signed integer).</summary>
    public static USharp.Core.Runtime.USharpType Long => _long;

    /// <summary>The U# <c>float</c> type (32-bit floating-point).</summary>
    public static USharp.Core.Runtime.USharpType Float => _float;

    /// <summary>The U# <c>double</c> type (64-bit floating-point).</summary>
    public static USharp.Core.Runtime.USharpType Double => _double;

    /// <summary>The U# <c>bool</c> type.</summary>
    public static USharp.Core.Runtime.USharpType Bool => _bool;

    /// <summary>The U# <c>string</c> type.</summary>
    public static USharp.Core.Runtime.USharpType String => _string;

    /// <summary>The U# <c>void</c> type.</summary>
    public static USharp.Core.Runtime.USharpType Void => _void;

    /// <summary>The U# <c>any</c> (dynamic) type.</summary>
    public static USharp.Core.Runtime.USharpType Any => _any;

    /// <summary>Returns all well-known types.</summary>
    public static IEnumerable<USharp.Core.Runtime.USharpType> All()
    {
        yield return _int;
        yield return _long;
        yield return _float;
        yield return _double;
        yield return _bool;
        yield return _string;
        yield return _void;
        yield return _any;
    }
}
