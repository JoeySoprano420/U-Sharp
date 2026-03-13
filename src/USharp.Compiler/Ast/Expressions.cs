namespace USharp.Compiler.Ast;

public abstract class ExpressionNode : AstNode
{
    protected ExpressionNode(SourceLocation location) : base(location) { }
}

public sealed class IdentifierExpr : ExpressionNode
{
    public string Name { get; }

    public IdentifierExpr(string name, SourceLocation location) : base(location)
    {
        Name = name;
    }
}

public sealed class LiteralExpr : ExpressionNode
{
    public object? Value { get; }
    public string RawText { get; }

    public LiteralExpr(object? value, string rawText, SourceLocation location) : base(location)
    {
        Value = value;
        RawText = rawText;
    }
}

public sealed class BinaryExpr : ExpressionNode
{
    public ExpressionNode Left { get; }
    public string Op { get; }
    public ExpressionNode Right { get; }

    public BinaryExpr(ExpressionNode left, string op, ExpressionNode right, SourceLocation location)
        : base(location)
    {
        Left = left;
        Op = op;
        Right = right;
    }
}

public sealed class UnaryExpr : ExpressionNode
{
    public string Op { get; }
    public ExpressionNode Operand { get; }

    public UnaryExpr(string op, ExpressionNode operand, SourceLocation location) : base(location)
    {
        Op = op;
        Operand = operand;
    }
}

public sealed class MemberAccessExpr : ExpressionNode
{
    public ExpressionNode Object { get; }
    public string Member { get; }

    public MemberAccessExpr(ExpressionNode obj, string member, SourceLocation location) : base(location)
    {
        Object = obj;
        Member = member;
    }
}

public sealed class CallExpr : ExpressionNode
{
    public ExpressionNode Callee { get; }
    public List<ExpressionNode> Arguments { get; }

    public CallExpr(ExpressionNode callee, List<ExpressionNode> arguments, SourceLocation location)
        : base(location)
    {
        Callee = callee;
        Arguments = arguments;
    }
}

public sealed class RangeExpr : ExpressionNode
{
    public ExpressionNode Start { get; }
    public ExpressionNode End { get; }

    public RangeExpr(ExpressionNode start, ExpressionNode end, SourceLocation location) : base(location)
    {
        Start = start;
        End = end;
    }
}
