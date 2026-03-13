namespace USharp.Compiler.Pipeline;

using USharp.Compiler.Lexer;
using USharp.Compiler.Rewriter;
using USharp.Compiler.Emitter;
using USharp.Compiler.Backend;
using USharp.Compiler.Diagnostics;

public sealed class CompilationPipeline
{
    public CompilationResult Compile(string source, CompilationOptions? options = null)
    {
        options ??= new CompilationOptions();
        var allDiagnostics = new List<Diagnostic>();

        try
        {
            // Lex
            var lexer = new Lexer(source);
            var tokens = lexer.Tokenize();

            // Parse
            var parser = new USharp.Compiler.Parser.Parser(tokens);
            var ast = parser.Parse();
            allDiagnostics.AddRange(parser.Diagnostics);

            if (allDiagnostics.Any(d => d.Severity == DiagnosticSeverity.Error))
                return new CompilationResult(false, null, null, allDiagnostics);

            // Semantic analysis
            var analyzer = new Semantics.SemanticAnalyzer();
            analyzer.Analyze(ast);
            allDiagnostics.AddRange(analyzer.Diagnostics);

            // Rewrite/optimize
            if (options.OptimizeEnabled)
            {
                var rewriter = new RewriteEngine();
                ast = rewriter.Rewrite(ast);
            }

            // Emit C#
            var emitter = new CSharpEmitter();
            var csharp = emitter.Emit(ast);

            if (!options.EmitAssembly)
                return new CompilationResult(true, csharp, null, allDiagnostics);

            // Roslyn compile
            var backend = new RoslynBackend();
            var (success, assembly, backendErrors) = backend.Compile(csharp, options.AssemblyName);

            foreach (var err in backendErrors)
                allDiagnostics.Add(Diagnostic.Error(DiagnosticCode.USP1002, err, 0, 0));

            if (options.OutputPath != null && success && assembly != null)
                File.WriteAllBytes(options.OutputPath, assembly);

            return new CompilationResult(success, csharp, assembly, allDiagnostics);
        }
        catch (Exception ex)
        {
            allDiagnostics.Add(Diagnostic.Error(DiagnosticCode.USP1001,
                $"Internal compiler error: {ex.Message}", 0, 0));
            return new CompilationResult(false, null, null, allDiagnostics);
        }
    }
}
