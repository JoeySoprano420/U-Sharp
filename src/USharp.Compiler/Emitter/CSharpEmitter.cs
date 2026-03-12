namespace USharp.Compiler.Emitter;

using System.Text;
using USharp.Compiler.Ast;

public sealed class CSharpEmitter
{
    private readonly StringBuilder _sb = new();
    private int _indent = 0;
    private const string IndentStr = "    ";

    public string Emit(ProgramNode program)
    {
        _sb.Clear();
        _indent = 0;

        _sb.AppendLine("using System;");
        _sb.AppendLine("using System.Collections.Generic;");
        _sb.AppendLine("using System.Linq;");
        _sb.AppendLine();

        foreach (var decl in program.Declarations)
            EmitTopLevel(decl);

        return _sb.ToString();
    }

    private void EmitTopLevel(AstNode node)
    {
        switch (node)
        {
            case ModuleDecl m:
                _sb.AppendLine($"namespace {m.Name};");
                _sb.AppendLine();
                break;
            case ClassDecl c:
                EmitClass(c);
                break;
            case FnDecl f:
                EmitFn(f);
                break;
            case StatementNode s:
                EmitStatement(s);
                break;
        }
    }

    private void EmitClass(ClassDecl c)
    {
        AppendIndent();
        _sb.AppendLine($"class {c.Name}");
        AppendIndent();
        _sb.AppendLine("{");
        _indent++;
        foreach (var m in c.Members)
        {
            switch (m)
            {
                case FieldDecl f:
                    EmitField(f);
                    break;
                case FnDecl fn:
                    EmitFn(fn);
                    break;
            }
        }
        _indent--;
        AppendIndent();
        _sb.AppendLine("}");
        _sb.AppendLine();
    }

    private void EmitField(FieldDecl f)
    {
        AppendIndent();
        if (f.IsReadonly) _sb.Append("readonly ");
        var typeName = f.Type != null ? EmitType(f.Type) : "var";
        _sb.Append($"{typeName} {f.Name}");
        if (f.Initializer != null)
        {
            _sb.Append(" = ");
            _sb.Append(EmitExpr(f.Initializer));
        }
        _sb.AppendLine(";");
    }

    private void EmitFn(FnDecl f)
    {
        AppendIndent();
        var returnType = EmitType(f.ReturnType);
        var name = ToPascalCase(f.Name);
        var parameters = string.Join(", ", f.Parameters.Select(p => $"{EmitType(p.Type)} {p.Name}"));
        _sb.AppendLine($"{returnType} {name}({parameters})");
        AppendIndent();
        _sb.AppendLine("{");
        _indent++;
        foreach (var stmt in f.Body)
            EmitStatement(stmt);
        _indent--;
        AppendIndent();
        _sb.AppendLine("}");
        _sb.AppendLine();
    }

    private void EmitStatement(StatementNode stmt)
    {
        switch (stmt)
        {
            case VarDeclStatement v:
                EmitVarDecl(v);
                break;
            case PrintStatement p:
                AppendIndent();
                _sb.AppendLine($"Console.WriteLine({EmitExpr(p.Value)});");
                break;
            case SendStatement s:
                AppendIndent();
                _sb.AppendLine($"Send{ToPascalCase(s.Target)}();");
                break;
            case ReturnStatement r:
                AppendIndent();
                _sb.Append("return");
                if (r.Value != null) { _sb.Append(" "); _sb.Append(EmitExpr(r.Value)); }
                _sb.AppendLine(";");
                break;
            case IfStatement i:
                EmitIf(i);
                break;
            case EachStatement e:
                EmitEach(e);
                break;
            case ForStatement fs:
                EmitFor(fs);
                break;
            case WhileStatement w:
                EmitWhile(w);
                break;
            case ExpressionStatement es:
                AppendIndent();
                _sb.AppendLine($"{EmitExpr(es.Expression)};");
                break;
            case PipelineStatement ps:
                EmitPipeline(ps);
                break;
        }
    }

    private void EmitVarDecl(VarDeclStatement v)
    {
        AppendIndent();
        if (v.IsReadonly) _sb.Append("readonly ");
        _sb.Append(v.Type != null ? EmitType(v.Type) : "var");
        _sb.Append($" {v.Name}");
        if (v.Initializer != null)
        {
            _sb.Append(" = ");
            _sb.Append(EmitExpr(v.Initializer));
        }
        _sb.AppendLine(";");
    }

    private void EmitIf(IfStatement i)
    {
        AppendIndent();
        _sb.AppendLine($"if ({EmitExpr(i.Condition)})");
        AppendIndent();
        _sb.AppendLine("{");
        _indent++;
        foreach (var s in i.Then.Statements) EmitStatement(s);
        _indent--;
        AppendIndent();
        if (i.Else != null)
        {
            _sb.AppendLine("} else {");
            _indent++;
            foreach (var s in i.Else.Statements) EmitStatement(s);
            _indent--;
            AppendIndent();
            _sb.AppendLine("}");
        }
        else
        {
            _sb.AppendLine("}");
        }
    }

    private void EmitEach(EachStatement e)
    {
        AppendIndent();
        _sb.AppendLine($"foreach (var {e.Variable} in {EmitExpr(e.Collection)})");
        AppendIndent();
        _sb.AppendLine("{");
        _indent++;
        foreach (var s in e.Body.Statements) EmitStatement(s);
        _indent--;
        AppendIndent();
        _sb.AppendLine("}");
    }

    private void EmitFor(ForStatement f)
    {
        AppendIndent();
        _sb.AppendLine($"for (int {f.Variable} = {EmitExpr(f.Start)}; {f.Variable} < {EmitExpr(f.End)}; {f.Variable}++)");
        AppendIndent();
        _sb.AppendLine("{");
        _indent++;
        foreach (var s in f.Body.Statements) EmitStatement(s);
        _indent--;
        AppendIndent();
        _sb.AppendLine("}");
    }

    private void EmitWhile(WhileStatement w)
    {
        AppendIndent();
        _sb.AppendLine($"while ({EmitExpr(w.Condition)})");
        AppendIndent();
        _sb.AppendLine("{");
        _indent++;
        foreach (var s in w.Body.Statements) EmitStatement(s);
        _indent--;
        AppendIndent();
        _sb.AppendLine("}");
    }

    private void EmitPipeline(PipelineStatement ps)
    {
        AppendIndent();
        var chain = new StringBuilder();
        chain.Append(EmitExpr(ps.Source));
        foreach (var stage in ps.Stages)
        {
            switch (stage.Operation)
            {
                case "filter":
                    var filterArg = stage.Argument != null ? EmitExpr(stage.Argument) : "x";
                    chain.Append($".Where(x => x.{ToPascalCase(filterArg)})");
                    break;
                case "map":
                    var mapArg = stage.Argument != null ? EmitExpr(stage.Argument) : "x";
                    chain.Append($".Select(x => x.{ToPascalCase(mapArg)})");
                    break;
                case "sort":
                    var sortArg = stage.Argument != null ? EmitExpr(stage.Argument) : "x";
                    chain.Append($".OrderBy(x => x.{ToPascalCase(sortArg)})");
                    break;
            }
        }
        _sb.AppendLine($"{chain};");
    }

    private string EmitExpr(ExpressionNode expr) => expr switch
    {
        LiteralExpr l => l.RawText,
        IdentifierExpr id => id.Name,
        BinaryExpr b => $"{EmitExpr(b.Left)} {b.Op} {EmitExpr(b.Right)}",
        UnaryExpr u => $"{u.Op}{EmitExpr(u.Operand)}",
        MemberAccessExpr m => $"{EmitExpr(m.Object)}.{ToPascalCase(m.Member)}",
        CallExpr c => $"{EmitExpr(c.Callee)}({string.Join(", ", c.Arguments.Select(EmitExpr))})",
        RangeExpr r => $"{EmitExpr(r.Start)}..{EmitExpr(r.End)}",
        _ => ""
    };

    private static string EmitType(TypeExprNode type)
    {
        var name = type.Name switch
        {
            "int" => "int",
            "long" => "long",
            "float" => "float",
            "double" => "double",
            "bool" => "bool",
            "string" => "string",
            "void" => "void",
            "any" => "object",
            "list" => "List",
            "map" => "Dictionary",
            _ => type.Name
        };

        if (type.TypeArgs.Count > 0)
            return $"{name}<{string.Join(", ", type.TypeArgs.Select(EmitType))}>";

        return name;
    }

    private static string ToPascalCase(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        return char.ToUpper(name[0]) + name[1..];
    }

    private void AppendIndent()
    {
        for (int i = 0; i < _indent; i++)
            _sb.Append(IndentStr);
    }
}
