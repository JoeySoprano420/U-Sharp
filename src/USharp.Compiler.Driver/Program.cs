using USharp.Compiler.Pipeline;
using USharp.Compiler.Diagnostics;

if (args.Length < 2)
{
    PrintUsage();
    return 1;
}

var command = args[0].ToLowerInvariant();
var file = args[1];

if (!File.Exists(file))
{
    Console.Error.WriteLine($"Error: File '{file}' not found.");
    return 1;
}

var source = File.ReadAllText(file);

switch (command)
{
    case "compile":
    {
        string? outputPath = null;
        for (int i = 2; i < args.Length - 1; i++)
        {
            if (args[i] == "-o") outputPath = args[i + 1];
        }
        outputPath ??= Path.ChangeExtension(file, ".dll");
        var options = new CompilationOptions
        {
            OutputPath = outputPath,
            AssemblyName = Path.GetFileNameWithoutExtension(file)
        };
        var result = new CompilationPipeline().Compile(source, options);
        PrintDiagnostics(result.Diagnostics);
        if (result.Success) Console.WriteLine($"Compiled to {outputPath}");
        return result.Success ? 0 : 1;
    }
    case "rewrite":
    {
        var options = new CompilationOptions { EmitAssembly = false };
        var result = new CompilationPipeline().Compile(source, options);
        PrintDiagnostics(result.Diagnostics);
        if (result.CSharpSource != null) Console.Write(result.CSharpSource);
        return result.Success ? 0 : 1;
    }
    case "check":
    {
        var options = new CompilationOptions { EmitAssembly = false };
        var result = new CompilationPipeline().Compile(source, options);
        PrintDiagnostics(result.Diagnostics);
        if (!result.Diagnostics.Any())
            Console.WriteLine("No diagnostics.");
        return result.Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error) ? 1 : 0;
    }
    case "run":
    {
        var options = new CompilationOptions
        {
            EmitAssembly = true,
            AssemblyName = Path.GetFileNameWithoutExtension(file)
        };
        var result = new CompilationPipeline().Compile(source, options);
        PrintDiagnostics(result.Diagnostics);
        if (result.Success && result.Assembly != null)
        {
            var asm = System.Reflection.Assembly.Load(result.Assembly);
            var entryPoint = asm.EntryPoint;
            if (entryPoint != null)
                entryPoint.Invoke(null,
                    entryPoint.GetParameters().Length == 1
                        ? new object?[] { Array.Empty<string>() }
                        : null);
            else
                Console.Error.WriteLine("No entry point found.");
        }
        return result.Success ? 0 : 1;
    }
    default:
        Console.Error.WriteLine($"Unknown command: {command}");
        PrintUsage();
        return 1;
}

static void PrintUsage()
{
    Console.WriteLine("Usage: usp <command> <file.usp> [options]");
    Console.WriteLine("Commands:");
    Console.WriteLine("  compile <file.usp> [-o output.dll]  Compile to assembly");
    Console.WriteLine("  rewrite <file.usp>                  Show generated C#");
    Console.WriteLine("  check   <file.usp>                  Show diagnostics only");
    Console.WriteLine("  run     <file.usp>                  Compile and execute");
}

static void PrintDiagnostics(IReadOnlyList<Diagnostic> diagnostics)
{
    foreach (var d in diagnostics)
        Console.Error.WriteLine(d.ToString());
}
