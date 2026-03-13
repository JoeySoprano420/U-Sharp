namespace USharp.Compiler.Semantics;

using USharp.Compiler.Ast;
using USharp.Compiler.Diagnostics;

public sealed class SemanticAnalyzer
{
    private readonly List<Diagnostic> _diagnostics = new();
    private SymbolTable _scope = new();

    public IReadOnlyList<Diagnostic> Diagnostics => _diagnostics;

    public void Analyze(ProgramNode program)
    {
        foreach (var decl in program.Declarations)
            AnalyzeNode(decl);
    }

    private void AnalyzeNode(AstNode node)
    {
        switch (node)
        {
            case ModuleDecl: break;
            case ClassDecl c: AnalyzeClass(c); break;
            case FnDecl f: AnalyzeFn(f); break;
            case FieldDecl fd: AnalyzeField(fd); break;
            case StatementNode s: AnalyzeStatement(s); break;
        }
    }

    private void AnalyzeClass(ClassDecl c)
    {
        _scope.Define(new Symbol(c.Name, SymbolKind.Class, c.Name));
        var prev = _scope;
        _scope = _scope.CreateChild();
        foreach (var m in c.Members) AnalyzeNode(m);
        _scope = prev;
    }

    private void AnalyzeFn(FnDecl f)
    {
        _scope.Define(new Symbol(f.Name, SymbolKind.Function, f.ReturnType.Name));
        var prev = _scope;
        _scope = _scope.CreateChild();
        foreach (var p in f.Parameters)
            _scope.Define(new Symbol(p.Name, SymbolKind.Parameter, p.Type.Name));
        foreach (var s in f.Body) AnalyzeStatement(s);
        _scope = prev;
    }

    private void AnalyzeField(FieldDecl f)
    {
        var typeName = f.Type?.Name ?? "object";
        _scope.Define(new Symbol(f.Name, SymbolKind.Field, typeName, f.IsReadonly));
        if (f.Initializer != null) AnalyzeExpr(f.Initializer);
    }

    private void AnalyzeStatement(StatementNode stmt)
    {
        switch (stmt)
        {
            case VarDeclStatement v:
                if (v.Initializer != null) AnalyzeExpr(v.Initializer);
                _scope.Define(new Symbol(v.Name, SymbolKind.Variable, v.Type?.Name ?? "var", v.IsReadonly));
                break;
            case PrintStatement p:
                AnalyzeExpr(p.Value);
                break;
            case ReturnStatement r:
                if (r.Value != null) AnalyzeExpr(r.Value);
                break;
            case IfStatement i:
                AnalyzeExpr(i.Condition);
                foreach (var s in i.Then.Statements) AnalyzeStatement(s);
                if (i.Else != null) foreach (var s in i.Else.Statements) AnalyzeStatement(s);
                break;
            case EachStatement e:
                AnalyzeExpr(e.Collection);
                var prevE = _scope;
                _scope = _scope.CreateChild();
                _scope.Define(new Symbol(e.Variable, SymbolKind.Variable, "var"));
                foreach (var s in e.Body.Statements) AnalyzeStatement(s);
                _scope = prevE;
                break;
            case ForStatement fs:
                AnalyzeExpr(fs.Start);
                AnalyzeExpr(fs.End);
                var prevF = _scope;
                _scope = _scope.CreateChild();
                _scope.Define(new Symbol(fs.Variable, SymbolKind.Variable, "int"));
                foreach (var s in fs.Body.Statements) AnalyzeStatement(s);
                _scope = prevF;
                break;
            case WhileStatement w:
                AnalyzeExpr(w.Condition);
                foreach (var s in w.Body.Statements) AnalyzeStatement(s);
                break;
            case ExpressionStatement es:
                AnalyzeExpr(es.Expression);
                break;
            case PipelineStatement ps:
                AnalyzeExpr(ps.Source);
                break;
        }
    }

    private void AnalyzeExpr(ExpressionNode expr)
    {
        switch (expr)
        {
            case IdentifierExpr:
                // Symbol resolution is lenient - many identifiers are external references
                break;
            case BinaryExpr b:
                AnalyzeExpr(b.Left);
                AnalyzeExpr(b.Right);
                break;
            case UnaryExpr u:
                AnalyzeExpr(u.Operand);
                break;
            case MemberAccessExpr m:
                AnalyzeExpr(m.Object);
                break;
            case CallExpr c:
                AnalyzeExpr(c.Callee);
                foreach (var a in c.Arguments) AnalyzeExpr(a);
                break;
        }
    }
}
