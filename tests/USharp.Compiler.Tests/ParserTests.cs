namespace USharp.Compiler.Tests;

using USharp.Compiler.Lexer;
using USharp.Compiler.Parser;
using USharp.Compiler.Ast;

public class ParserTests
{
    private static ProgramNode Parse(string source)
    {
        var tokens = new Lexer(source).Tokenize();
        return new Parser(tokens).Parse();
    }

    [Fact]
    public void Parses_ModuleDecl()
    {
        var program = Parse("module MyApp");
        Assert.Contains(program.Declarations, d => d is ModuleDecl m && m.Name == "MyApp");
    }

    [Fact]
    public void Parses_ClassDecl()
    {
        var program = Parse("class User\n    age empty int\n");
        Assert.Contains(program.Declarations, d => d is ClassDecl c && c.Name == "User");
        var cls = program.Declarations.OfType<ClassDecl>().First();
        Assert.Contains(cls.Members, m => m is FieldDecl f && f.Name == "age");
    }

    [Fact]
    public void Parses_FnDecl()
    {
        var program = Parse("fn greet(msg: string): void\n    print msg\n");
        Assert.Contains(program.Declarations, d => d is FnDecl f && f.Name == "greet");
        var fn = program.Declarations.OfType<FnDecl>().First();
        Assert.Single(fn.Parameters);
        Assert.Equal("msg", fn.Parameters[0].Name);
        Assert.Equal("string", fn.Parameters[0].Type.Name);
    }

    [Fact]
    public void Parses_PrintStatement()
    {
        var program = Parse("fn main(): void\n    print \"hello\"\n");
        var fn = program.Declarations.OfType<FnDecl>().First();
        Assert.Contains(fn.Body, s => s is PrintStatement);
    }

    [Fact]
    public void Parses_IfStatement()
    {
        var program = Parse("fn main(): void\n    if x\n        print y\n");
        var fn = program.Declarations.OfType<FnDecl>().First();
        Assert.Contains(fn.Body, s => s is IfStatement);
    }

    [Fact]
    public void Parses_EachStatement()
    {
        var program = Parse("fn main(): void\n    each item in items\n        print item\n");
        var fn = program.Declarations.OfType<FnDecl>().First();
        Assert.Contains(fn.Body, s => s is EachStatement);
    }

    [Fact]
    public void Parses_ForStatement()
    {
        var program = Parse("fn main(): void\n    for i in 0..10\n        print i\n");
        var fn = program.Declarations.OfType<FnDecl>().First();
        Assert.Contains(fn.Body, s => s is ForStatement);
    }

    [Fact]
    public void Parses_WhileStatement()
    {
        var program = Parse("fn main(): void\n    while running\n        print x\n");
        var fn = program.Declarations.OfType<FnDecl>().First();
        Assert.Contains(fn.Body, s => s is WhileStatement);
    }

    [Fact]
    public void Parses_ReturnStatement()
    {
        var program = Parse("fn add(a: int, b: int): int\n    return a + b\n");
        var fn = program.Declarations.OfType<FnDecl>().First();
        Assert.Contains(fn.Body, s => s is ReturnStatement r && r.Value != null);
    }

    [Fact]
    public void Parses_VarDecl_Empty()
    {
        var program = Parse("fn main(): void\n    count empty int\n");
        var fn = program.Declarations.OfType<FnDecl>().First();
        Assert.Contains(fn.Body, s => s is VarDeclStatement v && v.Name == "count" &&
            v.Type != null && v.Type.Name == "int");
    }

    [Fact]
    public void Parses_VarDecl_Retain()
    {
        var program = Parse("fn main(): void\n    token retain string = \"abc\"\n");
        var fn = program.Declarations.OfType<FnDecl>().First();
        Assert.Contains(fn.Body, s => s is VarDeclStatement v && v.Name == "token" && v.IsReadonly);
    }

    [Fact]
    public void Parses_SendStatement()
    {
        var program = Parse("fn main(): void\n    send welcome\n");
        var fn = program.Declarations.OfType<FnDecl>().First();
        Assert.Contains(fn.Body, s => s is SendStatement ss && ss.Target == "welcome");
    }

    [Fact]
    public void Parses_MemberAccess()
    {
        var program = Parse("fn main(): void\n    print user.name\n");
        var fn = program.Declarations.OfType<FnDecl>().First();
        var print = fn.Body.OfType<PrintStatement>().First();
        Assert.IsType<MemberAccessExpr>(print.Value);
    }
}
