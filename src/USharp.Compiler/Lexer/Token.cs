namespace USharp.Compiler.Lexer;

public readonly struct Token
{
    public TokenKind Kind { get; }
    public string Text { get; }
    public int Line { get; }
    public int Column { get; }

    public Token(TokenKind kind, string text, int line, int column)
    {
        Kind = kind;
        Text = text;
        Line = line;
        Column = column;
    }

    public override string ToString() => $"{Kind}({Text}) at {Line}:{Column}";
}
