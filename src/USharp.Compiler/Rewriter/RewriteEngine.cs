namespace USharp.Compiler.Rewriter;

using USharp.Compiler.Ast;

public sealed class RewriteEngine
{
    public ProgramNode Rewrite(ProgramNode program)
    {
        var newDecls = program.Declarations.Select(RewriteNode).ToList();
        return new ProgramNode(newDecls, program.Location);
    }

    private AstNode RewriteNode(AstNode node) => node switch
    {
        FnDecl f => new FnDecl(f.Name, f.Parameters, f.ReturnType,
            f.Body.Select(RewriteStatement).OfType<StatementNode>().ToList(), f.Location),
        ClassDecl c => new ClassDecl(c.Name, c.Members.Select(RewriteNode).ToList(), c.Location),
        StatementNode s => RewriteStatement(s) ?? s,
        _ => node
    };

    private StatementNode? RewriteStatement(StatementNode stmt) => stmt switch
    {
        ExpressionStatement es => new ExpressionStatement(RewriteExpr(es.Expression), es.Location),
        PrintStatement ps => new PrintStatement(RewriteExpr(ps.Value), ps.Location),
        ReturnStatement rs => new ReturnStatement(
            rs.Value != null ? RewriteExpr(rs.Value) : null, rs.Location),
        IfStatement i => new IfStatement(
            RewriteExpr(i.Condition),
            new BlockStatement(
                i.Then.Statements.Select(RewriteStatement).OfType<StatementNode>().ToList(),
                i.Then.Location),
            i.Else != null ? new BlockStatement(
                i.Else.Statements.Select(RewriteStatement).OfType<StatementNode>().ToList(),
                i.Else.Location) : null,
            i.Location),
        VarDeclStatement v => new VarDeclStatement(v.Name, v.IsReadonly, v.Type,
            v.Initializer != null ? RewriteExpr(v.Initializer) : null, v.Location),
        WhileStatement w => new WhileStatement(
            RewriteExpr(w.Condition),
            new BlockStatement(
                w.Body.Statements.Select(RewriteStatement).OfType<StatementNode>().ToList(),
                w.Body.Location),
            w.Location),
        EachStatement e => new EachStatement(e.Variable, RewriteExpr(e.Collection),
            new BlockStatement(
                e.Body.Statements.Select(RewriteStatement).OfType<StatementNode>().ToList(),
                e.Body.Location),
            e.Location),
        ForStatement f => new ForStatement(f.Variable, RewriteExpr(f.Start), RewriteExpr(f.End),
            new BlockStatement(
                f.Body.Statements.Select(RewriteStatement).OfType<StatementNode>().ToList(),
                f.Body.Location),
            f.Location),
        _ => stmt
    };

    private ExpressionNode RewriteExpr(ExpressionNode expr)
    {
        switch (expr)
        {
            case BinaryExpr b:
            {
                var left = RewriteExpr(b.Left);
                var right = RewriteExpr(b.Right);

                // x + 0 => x
                if (b.Op == "+" && right is LiteralExpr { Value: int ri } && ri == 0)
                    return left;
                if (b.Op == "+" && left is LiteralExpr { Value: int li } && li == 0)
                    return right;

                // x * 1 => x
                if (b.Op == "*" && right is LiteralExpr { Value: int rk } && rk == 1)
                    return left;
                if (b.Op == "*" && left is LiteralExpr { Value: int lk } && lk == 1)
                    return right;

                // list.count > 0 => list.Any()
                if (b.Op == ">" && right is LiteralExpr { Value: int z } && z == 0
                    && left is MemberAccessExpr { Member: "count" } memberExpr)
                {
                    return new CallExpr(
                        new MemberAccessExpr(memberExpr.Object, "Any", expr.Location),
                        [],
                        expr.Location);
                }

                return new BinaryExpr(left, b.Op, right, b.Location);
            }
            case UnaryExpr u:
                return new UnaryExpr(u.Op, RewriteExpr(u.Operand), u.Location);
            case MemberAccessExpr m:
                return new MemberAccessExpr(RewriteExpr(m.Object), m.Member, m.Location);
            case CallExpr c:
                return new CallExpr(
                    RewriteExpr(c.Callee),
                    c.Arguments.Select(RewriteExpr).ToList(),
                    c.Location);
            default:
                return expr;
        }
    }
}
