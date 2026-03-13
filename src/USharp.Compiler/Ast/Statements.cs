namespace USharp.Compiler.Ast;

public abstract class StatementNode : AstNode
{
    protected StatementNode(SourceLocation location) : base(location) { }
}

public sealed class BlockStatement : StatementNode
{
    public List<StatementNode> Statements { get; }

    public BlockStatement(List<StatementNode> statements, SourceLocation location) : base(location)
    {
        Statements = statements;
    }
}

public sealed class ExpressionStatement : StatementNode
{
    public ExpressionNode Expression { get; }

    public ExpressionStatement(ExpressionNode expression, SourceLocation location) : base(location)
    {
        Expression = expression;
    }
}

public sealed class ReturnStatement : StatementNode
{
    public ExpressionNode? Value { get; }

    public ReturnStatement(ExpressionNode? value, SourceLocation location) : base(location)
    {
        Value = value;
    }
}

public sealed class PrintStatement : StatementNode
{
    public ExpressionNode Value { get; }

    public PrintStatement(ExpressionNode value, SourceLocation location) : base(location)
    {
        Value = value;
    }
}

public sealed class SendStatement : StatementNode
{
    public string Target { get; }

    public SendStatement(string target, SourceLocation location) : base(location)
    {
        Target = target;
    }
}

public sealed class IfStatement : StatementNode
{
    public ExpressionNode Condition { get; }
    public BlockStatement Then { get; }
    public BlockStatement? Else { get; }

    public IfStatement(ExpressionNode condition, BlockStatement then, BlockStatement? @else,
        SourceLocation location) : base(location)
    {
        Condition = condition;
        Then = then;
        Else = @else;
    }
}

public sealed class EachStatement : StatementNode
{
    public string Variable { get; }
    public ExpressionNode Collection { get; }
    public BlockStatement Body { get; }

    public EachStatement(string variable, ExpressionNode collection, BlockStatement body,
        SourceLocation location) : base(location)
    {
        Variable = variable;
        Collection = collection;
        Body = body;
    }
}

public sealed class ForStatement : StatementNode
{
    public string Variable { get; }
    public ExpressionNode Start { get; }
    public ExpressionNode End { get; }
    public BlockStatement Body { get; }

    public ForStatement(string variable, ExpressionNode start, ExpressionNode end,
        BlockStatement body, SourceLocation location) : base(location)
    {
        Variable = variable;
        Start = start;
        End = end;
        Body = body;
    }
}

public sealed class WhileStatement : StatementNode
{
    public ExpressionNode Condition { get; }
    public BlockStatement Body { get; }

    public WhileStatement(ExpressionNode condition, BlockStatement body, SourceLocation location)
        : base(location)
    {
        Condition = condition;
        Body = body;
    }
}

public sealed class VarDeclStatement : StatementNode
{
    public string Name { get; }
    public bool IsReadonly { get; }
    public TypeExprNode? Type { get; }
    public ExpressionNode? Initializer { get; }

    public VarDeclStatement(string name, bool isReadonly, TypeExprNode? type, ExpressionNode? initializer,
        SourceLocation location) : base(location)
    {
        Name = name;
        IsReadonly = isReadonly;
        Type = type;
        Initializer = initializer;
    }
}

public sealed class PipelineStatement : StatementNode
{
    public ExpressionNode Source { get; }
    public List<PipelineStage> Stages { get; }

    public PipelineStatement(ExpressionNode source, List<PipelineStage> stages, SourceLocation location)
        : base(location)
    {
        Source = source;
        Stages = stages;
    }
}

public sealed class PipelineStage : AstNode
{
    public string Operation { get; }
    public ExpressionNode? Argument { get; }

    public PipelineStage(string operation, ExpressionNode? argument, SourceLocation location)
        : base(location)
    {
        Operation = operation;
        Argument = argument;
    }
}
