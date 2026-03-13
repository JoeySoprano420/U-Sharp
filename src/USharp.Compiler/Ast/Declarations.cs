namespace USharp.Compiler.Ast;

public sealed class ProgramNode : AstNode
{
    public List<AstNode> Declarations { get; }

    public ProgramNode(List<AstNode> declarations, SourceLocation location) : base(location)
    {
        Declarations = declarations;
    }
}

public sealed class ModuleDecl : AstNode
{
    public string Name { get; }

    public ModuleDecl(string name, SourceLocation location) : base(location)
    {
        Name = name;
    }
}

public sealed class ClassDecl : AstNode
{
    public string Name { get; }
    public List<AstNode> Members { get; }

    public ClassDecl(string name, List<AstNode> members, SourceLocation location) : base(location)
    {
        Name = name;
        Members = members;
    }
}

public sealed class FnDecl : AstNode
{
    public string Name { get; }
    public List<ParameterNode> Parameters { get; }
    public TypeExprNode ReturnType { get; }
    public List<StatementNode> Body { get; }

    public FnDecl(string name, List<ParameterNode> parameters, TypeExprNode returnType,
        List<StatementNode> body, SourceLocation location) : base(location)
    {
        Name = name;
        Parameters = parameters;
        ReturnType = returnType;
        Body = body;
    }
}

public sealed class ParameterNode : AstNode
{
    public string Name { get; }
    public TypeExprNode Type { get; }

    public ParameterNode(string name, TypeExprNode type, SourceLocation location) : base(location)
    {
        Name = name;
        Type = type;
    }
}

public sealed class TypeExprNode : AstNode
{
    public string Name { get; }
    public List<TypeExprNode> TypeArgs { get; }

    public TypeExprNode(string name, List<TypeExprNode> typeArgs, SourceLocation location) : base(location)
    {
        Name = name;
        TypeArgs = typeArgs;
    }
}

public sealed class FieldDecl : AstNode
{
    public string Name { get; }
    public bool IsReadonly { get; }
    public TypeExprNode? Type { get; }
    public ExpressionNode? Initializer { get; }

    public FieldDecl(string name, bool isReadonly, TypeExprNode? type, ExpressionNode? initializer,
        SourceLocation location) : base(location)
    {
        Name = name;
        IsReadonly = isReadonly;
        Type = type;
        Initializer = initializer;
    }
}
