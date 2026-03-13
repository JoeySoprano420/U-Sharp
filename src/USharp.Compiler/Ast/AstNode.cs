namespace USharp.Compiler.Ast;

public readonly struct SourceLocation
{
    public int Line { get; }
    public int Column { get; }

    public SourceLocation(int line, int column)
    {
        Line = line;
        Column = column;
    }

    public override string ToString() => $"{Line}:{Column}";
}

public abstract class AstNode
{
    public SourceLocation Location { get; }

    protected AstNode(SourceLocation location)
    {
        Location = location;
    }
}
