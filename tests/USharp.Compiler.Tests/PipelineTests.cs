namespace USharp.Compiler.Tests;

using USharp.Compiler.Pipeline;
using USharp.Compiler.Diagnostics;

public class PipelineTests
{
    [Fact]
    public void Pipeline_ReturnsSuccess_ForValidSource()
    {
        var source = "module Test";
        var result = new CompilationPipeline().Compile(source,
            new CompilationOptions { EmitAssembly = false });
        Assert.DoesNotContain(result.Diagnostics, d => d.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void Pipeline_ProducesCSharpSource()
    {
        var source = "module MyApp";
        var result = new CompilationPipeline().Compile(source,
            new CompilationOptions { EmitAssembly = false });
        Assert.NotNull(result.CSharpSource);
        Assert.Contains("namespace MyApp;", result.CSharpSource);
    }

    [Fact]
    public void Pipeline_CompilesFunctionWithPrint()
    {
        var source = "fn main(): void\n    print \"hello\"\n";
        var result = new CompilationPipeline().Compile(source,
            new CompilationOptions { EmitAssembly = false });
        Assert.NotNull(result.CSharpSource);
        Assert.Contains("Console.WriteLine", result.CSharpSource);
    }

    [Fact]
    public void Pipeline_OptimizerRemovesAddZero()
    {
        var source = "fn main(): void\n    x: int = 5 + 0\n";
        var result = new CompilationPipeline().Compile(source,
            new CompilationOptions { EmitAssembly = false });
        Assert.NotNull(result.CSharpSource);
        Assert.DoesNotContain("5 + 0", result.CSharpSource);
    }

    [Fact]
    public void Pipeline_OptimizerRemovesMultiplyOne()
    {
        var source = "fn main(): void\n    x: int = 5 * 1\n";
        var result = new CompilationPipeline().Compile(source,
            new CompilationOptions { EmitAssembly = false });
        Assert.NotNull(result.CSharpSource);
        Assert.DoesNotContain("5 * 1", result.CSharpSource);
    }

    [Fact]
    public void Pipeline_RoslynCompiles_SimpleProgram()
    {
        var source = "fn Main(): void\n    print \"Hello, World!\"\n";
        var result = new CompilationPipeline().Compile(source,
            new CompilationOptions { EmitAssembly = true, AssemblyName = "TestProgram" });
        Assert.NotNull(result.CSharpSource);
    }

    [Fact]
    public void Pipeline_HandlesEmptySource()
    {
        var result = new CompilationPipeline().Compile("",
            new CompilationOptions { EmitAssembly = false });
        Assert.NotNull(result);
    }

    [Fact]
    public void Pipeline_ClassWithFields()
    {
        var source = "class Person\n    name retain string = \"Alice\"\n    age empty int\n";
        var result = new CompilationPipeline().Compile(source,
            new CompilationOptions { EmitAssembly = false });
        Assert.NotNull(result.CSharpSource);
        Assert.Contains("class Person", result.CSharpSource);
        Assert.Contains("readonly string name", result.CSharpSource);
        Assert.Contains("int age", result.CSharpSource);
    }

    [Fact]
    public void Pipeline_IfElse()
    {
        var source = "fn check(active: bool): void\n    if active\n        send welcome\n    else\n        send verification\n";
        var result = new CompilationPipeline().Compile(source,
            new CompilationOptions { EmitAssembly = false });
        Assert.NotNull(result.CSharpSource);
        Assert.Contains("if (active)", result.CSharpSource);
        Assert.Contains("SendWelcome()", result.CSharpSource);
        Assert.Contains("SendVerification()", result.CSharpSource);
    }
}
