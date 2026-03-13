namespace USharp.Compiler.Tests;

using USharp.Compiler.Lexer;
using USharp.Compiler.Parser;
using USharp.Compiler.Emitter;

public class EmitterTests
{
    private static string Emit(string source)
    {
        var tokens = new Lexer(source).Tokenize();
        var ast = new Parser(tokens).Parse();
        return new CSharpEmitter().Emit(ast);
    }

    [Fact]
    public void Emits_Namespace()
    {
        var cs = Emit("module MyApp");
        Assert.Contains("namespace MyApp;", cs);
    }

    [Fact]
    public void Emits_Class()
    {
        var cs = Emit("class User\n    age empty int\n");
        Assert.Contains("class User", cs);
        Assert.Contains("int age", cs);
    }

    [Fact]
    public void Emits_ReadonlyField()
    {
        var cs = Emit("class User\n    name retain string = \"Alice\"\n");
        Assert.Contains("readonly string name", cs);
        Assert.Contains("\"Alice\"", cs);
    }

    [Fact]
    public void Emits_Function_PascalCase()
    {
        var cs = Emit("fn greet(msg: string): void\n    print msg\n");
        Assert.Contains("void Greet(string msg)", cs);
    }

    [Fact]
    public void Emits_PrintStatement()
    {
        var cs = Emit("fn main(): void\n    print \"hello\"\n");
        Assert.Contains("Console.WriteLine(\"hello\")", cs);
    }

    [Fact]
    public void Emits_SendStatement()
    {
        var cs = Emit("fn main(): void\n    send welcome\n");
        Assert.Contains("SendWelcome();", cs);
    }

    [Fact]
    public void Emits_IfStatement()
    {
        var cs = Emit("fn main(): void\n    if x\n        print y\n");
        Assert.Contains("if (x)", cs);
        Assert.Contains("Console.WriteLine(y)", cs);
    }

    [Fact]
    public void Emits_ForeachLoop()
    {
        var cs = Emit("fn main(): void\n    each item in items\n        print item\n");
        Assert.Contains("foreach (var item in items)", cs);
    }

    [Fact]
    public void Emits_ForLoop()
    {
        var cs = Emit("fn main(): void\n    for i in 0..10\n        print i\n");
        Assert.Contains("for (int i = 0; i < 10; i++)", cs);
    }

    [Fact]
    public void Emits_WhileLoop()
    {
        var cs = Emit("fn main(): void\n    while running\n        print x\n");
        Assert.Contains("while (running)", cs);
    }

    [Fact]
    public void Emits_MemberAccess_PascalCase()
    {
        var cs = Emit("fn main(): void\n    print user.name\n");
        Assert.Contains("user.Name", cs);
    }

    [Fact]
    public void Emits_ReturnStatement()
    {
        var cs = Emit("fn add(a: int, b: int): int\n    return a + b\n");
        Assert.Contains("return a + b;", cs);
    }

    [Fact]
    public void Emits_Pipeline()
    {
        var cs = Emit("fn main(): void\n    users\n        filter active\n        map name\n        sort name\n");
        Assert.Contains(".Where(", cs);
        Assert.Contains(".Select(", cs);
        Assert.Contains(".OrderBy(", cs);
    }

    [Fact]
    public void Emits_VarDecl_Empty()
    {
        var cs = Emit("fn main(): void\n    count empty int\n");
        Assert.Contains("int count;", cs);
    }

    [Fact]
    public void Emits_VarDecl_Retain()
    {
        var cs = Emit("fn main(): void\n    token retain string = \"abc\"\n");
        Assert.Contains("readonly string token", cs);
    }

    [Fact]
    public void Emits_LogicalOperators()
    {
        var cs = Emit("fn main(): void\n    if a and b\n        print x\n");
        Assert.Contains("&&", cs);
    }

    [Fact]
    public void IncludesStandardUsings()
    {
        var cs = Emit("module Test");
        Assert.Contains("using System;", cs);
        Assert.Contains("using System.Collections.Generic;", cs);
        Assert.Contains("using System.Linq;", cs);
    }
}
