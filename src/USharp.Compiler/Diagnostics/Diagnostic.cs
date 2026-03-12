namespace USharp.Compiler.Diagnostics;

public sealed class Diagnostic
{
    public string Code { get; }
    public string Message { get; }
    public DiagnosticSeverity Severity { get; }
    public int Line { get; }
    public int Column { get; }
    public string? Suggestion { get; }

    private Diagnostic(string code, string message, DiagnosticSeverity severity,
        int line, int column, string? suggestion)
    {
        Code = code;
        Message = message;
        Severity = severity;
        Line = line;
        Column = column;
        Suggestion = suggestion;
    }

    public static Diagnostic Error(string code, string message, int line, int column,
        string? suggestion = null)
        => new(code, message, DiagnosticSeverity.Error, line, column, suggestion);

    public static Diagnostic Warning(string code, string message, int line, int column,
        string? suggestion = null)
        => new(code, message, DiagnosticSeverity.Warning, line, column, suggestion);

    public static Diagnostic Info(string code, string message, int line, int column,
        string? suggestion = null)
        => new(code, message, DiagnosticSeverity.Info, line, column, suggestion);

    public override string ToString()
    {
        var sug = Suggestion != null ? $" Suggestion: {Suggestion}" : "";
        return $"{Severity} {Code} ({Line},{Column}): {Message}{sug}";
    }
}
