using System.Text;
using System.Text.RegularExpressions;

namespace USharp.Text;

/// <summary>
/// Provides string manipulation utilities for U# programs.
/// </summary>
public static class USharpString
{
    /// <summary>Returns <see langword="true"/> if the string is null, empty, or whitespace.</summary>
    public static bool IsBlank(string? value) =>
        string.IsNullOrWhiteSpace(value);

    /// <summary>Reverses the characters in a string.</summary>
    public static string Reverse(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        var chars = value.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    /// <summary>Returns the string repeated <paramref name="count"/> times.</summary>
    public static string Repeat(string value, int count)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        if (count == 0) return string.Empty;
        var sb = new StringBuilder(value.Length * count);
        for (int i = 0; i < count; i++) sb.Append(value);
        return sb.ToString();
    }

    /// <summary>Returns the number of times <paramref name="substring"/> occurs in <paramref name="value"/>.</summary>
    public static int CountOccurrences(string value, string substring)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(substring);
        if (substring.Length == 0) return 0;
        int count = 0;
        int index = 0;
        while ((index = value.IndexOf(substring, index, StringComparison.Ordinal)) != -1)
        {
            count++;
            index += substring.Length;
        }
        return count;
    }

    /// <summary>
    /// Truncates <paramref name="value"/> to at most <paramref name="maxLength"/> characters,
    /// appending <paramref name="ellipsis"/> if truncated.
    /// </summary>
    public static string Truncate(string value, int maxLength, string ellipsis = "...")
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentOutOfRangeException.ThrowIfNegative(maxLength);
        if (value.Length <= maxLength) return value;
        return string.Concat(value.AsSpan(0, maxLength - ellipsis.Length), ellipsis);
    }

    /// <summary>Converts a string to Title Case.</summary>
    public static string ToTitleCase(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value.Length == 0) return value;
        var words = value.Split(' ');
        return string.Join(' ', words.Select(w =>
            w.Length == 0 ? w : char.ToUpper(w[0]) + w[1..].ToLower()));
    }

    /// <summary>Converts a string to camelCase.</summary>
    public static string ToCamelCase(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        var title = ToTitleCase(value).Replace(" ", "", StringComparison.Ordinal);
        if (title.Length == 0) return title;
        return char.ToLower(title[0]) + title[1..];
    }

    /// <summary>Converts a string to snake_case (lowercase, spaces replaced by underscores).</summary>
    public static string ToSnakeCase(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return Regex.Replace(value, @"([A-Z])", "_$1")
                    .TrimStart('_')
                    .Replace(' ', '_')
                    .ToLower();
    }

    /// <summary>Pads a string to <paramref name="width"/> characters by centering it.</summary>
    public static string PadCenter(string value, int width, char padChar = ' ')
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value.Length >= width) return value;
        int totalPad = width - value.Length;
        int leftPad = totalPad / 2;
        int rightPad = totalPad - leftPad;
        return new string(padChar, leftPad) + value + new string(padChar, rightPad);
    }

    /// <summary>Splits the string into lines.</summary>
    public static string[] SplitLines(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return value.Split(["\r\n", "\n", "\r"], StringSplitOptions.None);
    }

    /// <summary>Returns <see langword="true"/> if <paramref name="value"/> is a valid integer.</summary>
    public static bool IsInteger(string value) =>
        long.TryParse(value, out _);

    /// <summary>Returns <see langword="true"/> if <paramref name="value"/> is a valid decimal number.</summary>
    public static bool IsNumber(string value) =>
        double.TryParse(value, out _);

    /// <summary>Removes all occurrences of <paramref name="chars"/> from <paramref name="value"/>.</summary>
    public static string RemoveChars(string value, params char[] chars)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (chars.Length == 0) return value;
        var set = new HashSet<char>(chars);
        return new string(value.Where(c => !set.Contains(c)).ToArray());
    }
}

/// <summary>
/// A mutable string builder with a U#-friendly API.
/// </summary>
public sealed class USharpStringBuilder
{
    private readonly StringBuilder _inner = new();

    /// <summary>Appends text.</summary>
    public USharpStringBuilder Append(object? value)
    {
        _inner.Append(value);
        return this;
    }

    /// <summary>Appends a line.</summary>
    public USharpStringBuilder AppendLine(string? value = null)
    {
        _inner.AppendLine(value);
        return this;
    }

    /// <summary>Inserts text at the specified index.</summary>
    public USharpStringBuilder Insert(int index, string value)
    {
        _inner.Insert(index, value);
        return this;
    }

    /// <summary>Replaces all occurrences of <paramref name="oldValue"/> with <paramref name="newValue"/>.</summary>
    public USharpStringBuilder Replace(string oldValue, string newValue)
    {
        _inner.Replace(oldValue, newValue);
        return this;
    }

    /// <summary>Gets the current length.</summary>
    public int Length => _inner.Length;

    /// <summary>Clears the builder.</summary>
    public USharpStringBuilder Clear()
    {
        _inner.Clear();
        return this;
    }

    /// <inheritdoc />
    public override string ToString() => _inner.ToString();
}
