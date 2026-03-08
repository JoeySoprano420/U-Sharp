using USharp.IO.Files;
using Xunit;

namespace USharp.IO.Tests;

public sealed class USharpFileTests : IDisposable
{
    private readonly string _tempDir;

    public USharpFileTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "USharpIOTests_" + Guid.NewGuid());
        Directory.CreateDirectory(_tempDir);
    }

    [Fact]
    public void WriteAndRead_RoundTrip()
    {
        var path = Path.Combine(_tempDir, "test.txt");
        USharpFile.WriteAllText(path, "Hello, U#!");
        var content = USharpFile.ReadAllText(path);
        Assert.Equal("Hello, U#!", content);
    }

    [Fact]
    public async Task WriteAndReadAsync_RoundTrip()
    {
        var path = Path.Combine(_tempDir, "async.txt");
        await USharpFile.WriteAllTextAsync(path, "async content");
        var content = await USharpFile.ReadAllTextAsync(path);
        Assert.Equal("async content", content);
    }

    [Fact]
    public void Exists_ReturnsTrueForExistingFile()
    {
        var path = Path.Combine(_tempDir, "exists.txt");
        USharpFile.WriteAllText(path, "data");
        Assert.True(USharpFile.Exists(path));
    }

    [Fact]
    public void Exists_ReturnsFalseForMissingFile()
    {
        Assert.False(USharpFile.Exists(Path.Combine(_tempDir, "missing.txt")));
    }

    [Fact]
    public void Delete_RemovesFile()
    {
        var path = Path.Combine(_tempDir, "delete.txt");
        USharpFile.WriteAllText(path, "bye");
        USharpFile.Delete(path);
        Assert.False(File.Exists(path));
    }

    [Fact]
    public void ReadAllLines_ReturnsCorrectLines()
    {
        var path = Path.Combine(_tempDir, "lines.txt");
        USharpFile.WriteAllText(path, "line1\nline2\nline3");
        var lines = USharpFile.ReadAllLines(path);
        Assert.Equal(3, lines.Length);
    }

    [Fact]
    public void AppendText_AppendsToFile()
    {
        var path = Path.Combine(_tempDir, "append.txt");
        USharpFile.WriteAllText(path, "first");
        USharpFile.AppendText(path, " second");
        var content = USharpFile.ReadAllText(path);
        Assert.Equal("first second", content);
    }

    [Fact]
    public void Copy_CreatesDestinationFile()
    {
        var src = Path.Combine(_tempDir, "src.txt");
        var dst = Path.Combine(_tempDir, "dst.txt");
        USharpFile.WriteAllText(src, "data");
        USharpFile.Copy(src, dst);
        Assert.True(File.Exists(dst));
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }
}

public sealed class USharpDirectoryTests : IDisposable
{
    private readonly string _tempDir;

    public USharpDirectoryTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "USharpDirTests_" + Guid.NewGuid());
    }

    [Fact]
    public void Create_ThenExists_ReturnsTrue()
    {
        USharpDirectory.Create(_tempDir);
        Assert.True(USharpDirectory.Exists(_tempDir));
    }

    [Fact]
    public void Delete_RemovesDirectory()
    {
        USharpDirectory.Create(_tempDir);
        USharpDirectory.Delete(_tempDir, recursive: true);
        Assert.False(Directory.Exists(_tempDir));
    }

    [Fact]
    public void GetFiles_ReturnsFilesInDirectory()
    {
        USharpDirectory.Create(_tempDir);
        File.WriteAllText(Path.Combine(_tempDir, "a.txt"), "a");
        File.WriteAllText(Path.Combine(_tempDir, "b.txt"), "b");
        var files = USharpDirectory.GetFiles(_tempDir);
        Assert.Equal(2, files.Length);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }
}
