using USharp.Core.Exceptions;

namespace USharp.IO.Files;

/// <summary>
/// Provides high-level file I/O operations for U# programs.
/// </summary>
public static class USharpFile
{
    /// <summary>Reads all text from a file.</summary>
    public static string ReadAllText(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        try
        {
            return File.ReadAllText(path);
        }
        catch (Exception ex)
        {
            throw new USharpException($"Failed to read file '{path}'.", ex);
        }
    }

    /// <summary>Reads all text from a file asynchronously.</summary>
    public static async Task<string> ReadAllTextAsync(string path,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        try
        {
            return await File.ReadAllTextAsync(path, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new USharpException($"Failed to read file '{path}'.", ex);
        }
    }

    /// <summary>Reads all lines from a file.</summary>
    public static string[] ReadAllLines(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        try
        {
            return File.ReadAllLines(path);
        }
        catch (Exception ex)
        {
            throw new USharpException($"Failed to read lines from '{path}'.", ex);
        }
    }

    /// <summary>Reads all bytes from a file.</summary>
    public static byte[] ReadAllBytes(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        try
        {
            return File.ReadAllBytes(path);
        }
        catch (Exception ex)
        {
            throw new USharpException($"Failed to read bytes from '{path}'.", ex);
        }
    }

    /// <summary>Writes text to a file, creating or overwriting it.</summary>
    public static void WriteAllText(string path, string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        try
        {
            File.WriteAllText(path, content);
        }
        catch (Exception ex)
        {
            throw new USharpException($"Failed to write file '{path}'.", ex);
        }
    }

    /// <summary>Writes text to a file asynchronously.</summary>
    public static async Task WriteAllTextAsync(string path, string content,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        try
        {
            await File.WriteAllTextAsync(path, content, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new USharpException($"Failed to write file '{path}'.", ex);
        }
    }

    /// <summary>Appends text to an existing file (or creates it).</summary>
    public static void AppendText(string path, string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        try
        {
            File.AppendAllText(path, content);
        }
        catch (Exception ex)
        {
            throw new USharpException($"Failed to append to file '{path}'.", ex);
        }
    }

    /// <summary>Returns <see langword="true"/> if the file exists.</summary>
    public static bool Exists(string path) => File.Exists(path);

    /// <summary>Deletes the file at <paramref name="path"/> if it exists.</summary>
    public static void Delete(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (File.Exists(path)) File.Delete(path);
    }

    /// <summary>Copies a file from <paramref name="source"/> to <paramref name="destination"/>.</summary>
    public static void Copy(string source, string destination, bool overwrite = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(source);
        ArgumentException.ThrowIfNullOrWhiteSpace(destination);
        try
        {
            File.Copy(source, destination, overwrite);
        }
        catch (Exception ex)
        {
            throw new USharpException(
                $"Failed to copy '{source}' to '{destination}'.", ex);
        }
    }
}

/// <summary>
/// Provides directory-level operations for U# programs.
/// </summary>
public static class USharpDirectory
{
    /// <summary>Creates a directory (and any intermediate directories).</summary>
    public static void Create(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        Directory.CreateDirectory(path);
    }

    /// <summary>Returns <see langword="true"/> if the directory exists.</summary>
    public static bool Exists(string path) => Directory.Exists(path);

    /// <summary>Deletes a directory, optionally recursively.</summary>
    public static void Delete(string path, bool recursive = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (Directory.Exists(path)) Directory.Delete(path, recursive);
    }

    /// <summary>Returns all files in a directory matching an optional pattern.</summary>
    public static string[] GetFiles(string path, string searchPattern = "*",
        bool recursive = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        return Directory.GetFiles(path, searchPattern,
            recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
    }

    /// <summary>Returns all subdirectories in a directory.</summary>
    public static string[] GetDirectories(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        return Directory.GetDirectories(path);
    }
}
