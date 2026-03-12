namespace USharp.Compiler.Pipeline;

public sealed class CompilationOptions
{
    public string AssemblyName { get; init; } = "USharpOutput";
    public bool OptimizeEnabled { get; init; } = true;
    public bool EmitAssembly { get; init; } = true;
    public string? OutputPath { get; init; }
}
