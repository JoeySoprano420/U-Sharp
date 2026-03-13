namespace USharp.Compiler.Tests;

using USharp.Compiler.Lexer;

public class LexerTests
{
    private static List<Token> Lex(string source) => new Lexer(source).Tokenize();

    [Fact]
    public void Tokenizes_SimpleIdentifier()
    {
        var tokens = Lex("hello");
        Assert.Contains(tokens, t => t.Kind == TokenKind.Identifier && t.Text == "hello");
    }

    [Fact]
    public void Tokenizes_Keywords()
    {
        var tokens = Lex("module class fn if else each for while in return print send");
        Assert.Contains(tokens, t => t.Kind == TokenKind.Module);
        Assert.Contains(tokens, t => t.Kind == TokenKind.Class);
        Assert.Contains(tokens, t => t.Kind == TokenKind.Fn);
        Assert.Contains(tokens, t => t.Kind == TokenKind.If);
        Assert.Contains(tokens, t => t.Kind == TokenKind.Else);
        Assert.Contains(tokens, t => t.Kind == TokenKind.Each);
        Assert.Contains(tokens, t => t.Kind == TokenKind.For);
        Assert.Contains(tokens, t => t.Kind == TokenKind.While);
        Assert.Contains(tokens, t => t.Kind == TokenKind.In);
        Assert.Contains(tokens, t => t.Kind == TokenKind.Return);
        Assert.Contains(tokens, t => t.Kind == TokenKind.Print);
        Assert.Contains(tokens, t => t.Kind == TokenKind.Send);
    }

    [Fact]
    public void Tokenizes_Indentation()
    {
        var source = "if x\n    print y\n";
        var tokens = Lex(source);
        Assert.Contains(tokens, t => t.Kind == TokenKind.Indent);
        Assert.Contains(tokens, t => t.Kind == TokenKind.Dedent);
    }

    [Fact]
    public void Tokenizes_StringLiteral()
    {
        var tokens = Lex("\"hello world\"");
        Assert.Contains(tokens, t => t.Kind == TokenKind.StringLiteral && t.Text == "\"hello world\"");
    }

    [Fact]
    public void Tokenizes_IntLiteral()
    {
        var tokens = Lex("42");
        Assert.Contains(tokens, t => t.Kind == TokenKind.IntLiteral && t.Text == "42");
    }

    [Fact]
    public void Tokenizes_FloatLiteral()
    {
        var tokens = Lex("3.14");
        Assert.Contains(tokens, t => t.Kind == TokenKind.FloatLiteral && t.Text == "3.14");
    }

    [Fact]
    public void SkipsHashComments()
    {
        var tokens = Lex("# this is a comment\nhello");
        Assert.DoesNotContain(tokens, t => t.Text == "this");
        Assert.Contains(tokens, t => t.Kind == TokenKind.Identifier && t.Text == "hello");
    }

    [Fact]
    public void SkipsDashDashComments()
    {
        var tokens = Lex("-- comment\nhello");
        Assert.DoesNotContain(tokens, t => t.Text == "comment");
        Assert.Contains(tokens, t => t.Kind == TokenKind.Identifier && t.Text == "hello");
    }

    [Fact]
    public void Tokenizes_RangeOperator()
    {
        var tokens = Lex("0..10");
        Assert.Contains(tokens, t => t.Kind == TokenKind.DotDot);
    }

    [Fact]
    public void Tokenizes_BoolLiterals()
    {
        var tokens = Lex("true false");
        var bools = tokens.Where(t => t.Kind == TokenKind.BoolLiteral).ToList();
        Assert.Equal(2, bools.Count);
    }

    [Fact]
    public void Tokenizes_Operators()
    {
        var tokens = Lex("+ - * / % == != < <= > >=");
        Assert.Contains(tokens, t => t.Kind == TokenKind.Plus);
        Assert.Contains(tokens, t => t.Kind == TokenKind.Minus);
        Assert.Contains(tokens, t => t.Kind == TokenKind.Star);
        Assert.Contains(tokens, t => t.Kind == TokenKind.Slash);
        Assert.Contains(tokens, t => t.Kind == TokenKind.Percent);
        Assert.Contains(tokens, t => t.Kind == TokenKind.EqualsEquals);
        Assert.Contains(tokens, t => t.Kind == TokenKind.BangEquals);
        Assert.Contains(tokens, t => t.Kind == TokenKind.Lt);
        Assert.Contains(tokens, t => t.Kind == TokenKind.LtEquals);
        Assert.Contains(tokens, t => t.Kind == TokenKind.Gt);
        Assert.Contains(tokens, t => t.Kind == TokenKind.GtEquals);
    }

    [Fact]
    public void AlwaysEndsWithEof()
    {
        var tokens = Lex("hello");
        Assert.Equal(TokenKind.EndOfFile, tokens[^1].Kind);
    }

    [Fact]
    public void MultipleIndentLevels()
    {
        var source = "fn foo\n    if x\n        print y\n";
        var tokens = Lex(source);
        var indents = tokens.Count(t => t.Kind == TokenKind.Indent);
        var dedents = tokens.Count(t => t.Kind == TokenKind.Dedent);
        Assert.Equal(indents, dedents);
    }
}
