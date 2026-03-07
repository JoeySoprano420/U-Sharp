namespace USharp.IO.Console;

/// <summary>
/// Provides console I/O utilities for U# programs.
/// </summary>
public static class USharpConsole
{
    /// <summary>Writes a value to the console followed by a newline.</summary>
    public static void WriteLine(object? value = null) =>
        System.Console.WriteLine(value);

    /// <summary>Writes a formatted string to the console followed by a newline.</summary>
    public static void WriteLine(string format, params object?[] args) =>
        System.Console.WriteLine(format, args);

    /// <summary>Writes a value to the console without a trailing newline.</summary>
    public static void Write(object? value) =>
        System.Console.Write(value);

    /// <summary>Reads the next line of input from the console.</summary>
    public static string? ReadLine() => System.Console.ReadLine();

    /// <summary>Reads the next line of input, returning <paramref name="prompt"/> first.</summary>
    public static string? Prompt(string prompt)
    {
        System.Console.Write(prompt);
        return System.Console.ReadLine();
    }

    /// <summary>Reads a single character from the console.</summary>
    public static int Read() => System.Console.Read();

    /// <summary>Clears the console screen.</summary>
    public static void Clear() => System.Console.Clear();

    /// <summary>Sets the foreground color for subsequent console output.</summary>
    public static void SetColor(ConsoleColor color) =>
        System.Console.ForegroundColor = color;

    /// <summary>Resets the console color to its default.</summary>
    public static void ResetColor() => System.Console.ResetColor();

    /// <summary>Writes a value to the console in the specified color.</summary>
    public static void WriteColored(object? value, ConsoleColor color)
    {
        SetColor(color);
        Write(value);
        ResetColor();
    }

    /// <summary>Writes a value to the console in the specified color, followed by a newline.</summary>
    public static void WriteLineColored(object? value, ConsoleColor color)
    {
        SetColor(color);
        WriteLine(value);
        ResetColor();
    }
}
