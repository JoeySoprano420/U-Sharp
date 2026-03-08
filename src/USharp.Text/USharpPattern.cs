using System.Text.RegularExpressions;

namespace USharp.Text;

/// <summary>
/// Provides pattern-matching utilities using regular expressions for U# programs.
/// </summary>
public sealed class USharpPattern
{
    private readonly Regex _regex;

    /// <summary>Creates a new pattern from a regular expression string.</summary>
    public USharpPattern(string pattern, bool ignoreCase = false,
        bool multiline = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pattern);
        var options = RegexOptions.Compiled;
        if (ignoreCase) options |= RegexOptions.IgnoreCase;
        if (multiline) options |= RegexOptions.Multiline;
        _regex = new Regex(pattern, options, TimeSpan.FromSeconds(5));
    }

    /// <summary>Returns <see langword="true"/> if the pattern matches anywhere in <paramref name="input"/>.</summary>
    public bool IsMatch(string input) => _regex.IsMatch(input);

    /// <summary>Returns the first match, or <see langword="null"/> if none.</summary>
    public string? Match(string input)
    {
        var m = _regex.Match(input);
        return m.Success ? m.Value : null;
    }

    /// <summary>Returns all matches in <paramref name="input"/>.</summary>
    public IReadOnlyList<string> Matches(string input) =>
        _regex.Matches(input).Select(m => m.Value).ToList();

    /// <summary>
    /// Returns all named or positional capture groups from the first match.
    /// </summary>
    public IReadOnlyDictionary<string, string> Capture(string input)
    {
        var m = _regex.Match(input);
        if (!m.Success) return new Dictionary<string, string>();
        return m.Groups.Keys
            .Where(k => m.Groups[k].Success)
            .ToDictionary(k => k, k => m.Groups[k].Value);
    }

    /// <summary>Replaces all matches with <paramref name="replacement"/>.</summary>
    public string Replace(string input, string replacement) =>
        _regex.Replace(input, replacement);

    /// <summary>Splits <paramref name="input"/> on matches.</summary>
    public string[] Split(string input) => _regex.Split(input);

    /// <summary>Returns a pre-built pattern for validating email addresses.</summary>
    public static USharpPattern Email() =>
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ignoreCase: true);

    /// <summary>Returns a pre-built pattern for validating URLs (http/https).</summary>
    public static USharpPattern Url() =>
        new(@"^https?://[^\s/$.?#].[^\s]*$", ignoreCase: true);

    /// <summary>Returns a pre-built pattern for matching integers.</summary>
    public static USharpPattern Integer() => new(@"^-?\d+$");

    /// <summary>Returns a pre-built pattern for matching decimal numbers.</summary>
    public static USharpPattern Decimal() => new(@"^-?\d+(\.\d+)?$");
}
