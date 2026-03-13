namespace USharp.Compiler.Pipeline;

using USharp.Compiler.Diagnostics;

public sealed class CompilationResult
{
    public bool Success { get; }
    public string? CSharpSource { get; }
    public byte[]? Assembly { get; }
    public IReadOnlyList<Diagnostic> Diagnostics { get; }

    public CompilationResult(bool success, string? csharpSource, byte[]? assembly,
        IReadOnlyList<Diagnostic> diagnostics)
    {
        Success = success;
        CSharpSource = csharpSource;
        Assembly = assembly;
        Diagnostics = diagnostics;
    }
}
