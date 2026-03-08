using USharp.Text;
using Xunit;

namespace USharp.Text.Tests;

public sealed class USharpStringTests
{
    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("   ", true)]
    [InlineData("hello", false)]
    public void IsBlank_ReturnsCorrectResult(string? value, bool expected)
    {
        Assert.Equal(expected, USharpString.IsBlank(value));
    }

    [Fact]
    public void Reverse_ReturnsReversedString()
    {
        Assert.Equal("cba", USharpString.Reverse("abc"));
    }

    [Fact]
    public void Repeat_RepeatsString()
    {
        Assert.Equal("abababab", USharpString.Repeat("ab", 4));
    }

    [Fact]
    public void Repeat_ZeroTimes_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, USharpString.Repeat("hello", 0));
    }

    [Fact]
    public void CountOccurrences_ReturnsCount()
    {
        Assert.Equal(3, USharpString.CountOccurrences("abcabcabc", "abc"));
    }

    [Fact]
    public void Truncate_LongString_AddEllipsis()
    {
        var result = USharpString.Truncate("Hello, World!", 8);
        Assert.Equal("Hello...", result);
        Assert.Equal(8, result.Length);
    }

    [Fact]
    public void Truncate_ShortString_NoChange()
    {
        Assert.Equal("Hi", USharpString.Truncate("Hi", 10));
    }

    [Fact]
    public void ToTitleCase_CapitalizesWords()
    {
        Assert.Equal("Hello World", USharpString.ToTitleCase("hello world"));
    }

    [Fact]
    public void ToCamelCase_ConvertsProperly()
    {
        var result = USharpString.ToCamelCase("hello world");
        Assert.Equal("helloWorld", result);
    }

    [Fact]
    public void PadCenter_CentersString()
    {
        Assert.Equal("  hi  ", USharpString.PadCenter("hi", 6));
    }

    [Fact]
    public void SplitLines_ReturnsLines()
    {
        var lines = USharpString.SplitLines("line1\nline2\nline3");
        Assert.Equal(3, lines.Length);
    }

    [Theory]
    [InlineData("42", true)]
    [InlineData("-7", true)]
    [InlineData("3.14", false)]
    [InlineData("abc", false)]
    public void IsInteger_ReturnsCorrectResult(string value, bool expected)
    {
        Assert.Equal(expected, USharpString.IsInteger(value));
    }

    [Fact]
    public void RemoveChars_RemovesSpecifiedCharacters()
    {
        Assert.Equal("hll", USharpString.RemoveChars("hello", 'e', 'o'));
    }
}

public sealed class USharpStringBuilderTests
{
    [Fact]
    public void Append_BuildsString()
    {
        var sb = new USharpStringBuilder();
        sb.Append("Hello").Append(", ").Append("World!");
        Assert.Equal("Hello, World!", sb.ToString());
    }

    [Fact]
    public void AppendLine_AddsNewline()
    {
        var sb = new USharpStringBuilder();
        sb.AppendLine("Line 1").AppendLine("Line 2");
        Assert.Contains("Line 1", sb.ToString());
        Assert.Contains("Line 2", sb.ToString());
    }

    [Fact]
    public void Replace_ReplacesText()
    {
        var sb = new USharpStringBuilder();
        sb.Append("foo bar foo");
        sb.Replace("foo", "baz");
        Assert.Equal("baz bar baz", sb.ToString());
    }

    [Fact]
    public void Clear_ResetsBuilder()
    {
        var sb = new USharpStringBuilder();
        sb.Append("something");
        sb.Clear();
        Assert.Equal(0, sb.Length);
    }
}

public sealed class USharpPatternTests
{
    [Fact]
    public void IsMatch_ValidEmail_ReturnsTrue()
    {
        var pattern = USharpPattern.Email();
        Assert.True(pattern.IsMatch("user@example.com"));
    }

    [Fact]
    public void IsMatch_InvalidEmail_ReturnsFalse()
    {
        var pattern = USharpPattern.Email();
        Assert.False(pattern.IsMatch("not-an-email"));
    }

    [Fact]
    public void Match_ReturnsFirstMatch()
    {
        var pattern = new USharpPattern(@"\d+");
        Assert.Equal("123", pattern.Match("abc123def456"));
    }

    [Fact]
    public void Matches_ReturnsAllMatches()
    {
        var pattern = new USharpPattern(@"\d+");
        var matches = pattern.Matches("abc123def456ghi789");
        Assert.Equal(3, matches.Count);
    }

    [Fact]
    public void Replace_SubstitutesMatches()
    {
        var pattern = new USharpPattern(@"\d+");
        Assert.Equal("abc_def_", pattern.Replace("abc123def456", "_"));
    }

    [Fact]
    public void Integer_MatchesIntegerString()
    {
        Assert.True(USharpPattern.Integer().IsMatch("-42"));
        Assert.False(USharpPattern.Integer().IsMatch("3.14"));
    }
}
