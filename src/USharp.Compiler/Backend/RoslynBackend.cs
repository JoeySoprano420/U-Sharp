namespace USharp.Compiler.Backend;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

public sealed class RoslynBackend
{
    public (bool Success, byte[]? Assembly, IReadOnlyList<string> Errors) Compile(
        string csharpSource,
        string assemblyName = "USharpOutput")
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(csharpSource);
        var references = GetReferences();

        var compilation = CSharpCompilation.Create(
            assemblyName,
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.ConsoleApplication));

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (result.Success)
        {
            ms.Seek(0, SeekOrigin.Begin);
            return (true, ms.ToArray(), []);
        }

        var errors = result.Diagnostics
            .Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
            .Select(d => d.ToString())
            .ToList();
        return (false, null, errors);
    }

    private static List<MetadataReference> GetReferences()
    {
        var refs = new List<MetadataReference>();
        var trustedAssemblies = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;
        if (trustedAssemblies != null)
        {
            foreach (var path in trustedAssemblies.Split(Path.PathSeparator))
            {
                if (File.Exists(path))
                    refs.Add(MetadataReference.CreateFromFile(path));
            }
        }
        else
        {
            var runtimeDir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
            foreach (var dll in Directory.GetFiles(runtimeDir, "*.dll"))
            {
                try { refs.Add(MetadataReference.CreateFromFile(dll)); }
                catch (Exception) { /* skip assemblies that can't be loaded as metadata references */ }
            }
        }
        return refs;
    }
}
