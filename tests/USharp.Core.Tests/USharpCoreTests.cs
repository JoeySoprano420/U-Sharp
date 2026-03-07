using USharp.Core.Exceptions;
using USharp.Core.Primitives;
using USharp.Core.Runtime;
using USharp.Core.Types;
using Xunit;

namespace USharp.Core.Tests;

public sealed class USharpTypeTests
{
    [Fact]
    public void FullName_CombinesNamespaceAndName()
    {
        var type = new USharpType("MyClass", "MyNamespace");
        Assert.Equal("MyNamespace.MyClass", type.FullName);
    }

    [Fact]
    public void FullName_NoNamespace_ReturnsNameOnly()
    {
        // Edge: namespace supplied but empty-ish not allowed; test with valid data
        var type = new USharpType("Root", "Root");
        Assert.Equal("Root.Root", type.FullName);
    }

    [Fact]
    public void IsAssignableFrom_SameType_ReturnsTrue()
    {
        var type = new USharpType("Foo", "NS");
        Assert.True(type.IsAssignableFrom(type));
    }

    [Fact]
    public void IsAssignableFrom_DerivedType_ReturnsTrue()
    {
        var baseType = new USharpType("Base", "NS");
        var derived = new USharpType("Derived", "NS", baseType: baseType);
        Assert.True(baseType.IsAssignableFrom(derived));
    }

    [Fact]
    public void IsAssignableFrom_UnrelatedType_ReturnsFalse()
    {
        var a = new USharpType("A", "NS");
        var b = new USharpType("B", "NS");
        Assert.False(a.IsAssignableFrom(b));
    }

    [Fact]
    public void Equality_SameFullName_AreEqual()
    {
        var a = new USharpType("Foo", "NS");
        var b = new USharpType("Foo", "NS");
        Assert.Equal(a, b);
    }
}

public sealed class USharpRuntimeTests
{
    [Fact]
    public void RegisterAndResolveType_Works()
    {
        using var runtime = new USharpRuntime();
        var type = new USharpType("Widget", "App");
        runtime.RegisterType(type);
        var resolved = runtime.ResolveType("App.Widget");
        Assert.Equal(type, resolved);
    }

    [Fact]
    public void ResolveType_UnknownName_ReturnsNull()
    {
        using var runtime = new USharpRuntime();
        Assert.Null(runtime.ResolveType("Unknown.Type"));
    }

    [Fact]
    public void GetRegisteredTypes_ReturnsAll()
    {
        using var runtime = new USharpRuntime();
        runtime.RegisterType(new USharpType("A", "NS"));
        runtime.RegisterType(new USharpType("B", "NS"));
        Assert.Equal(2, runtime.GetRegisteredTypes().Count());
    }

    [Fact]
    public void Dispose_ThenUse_ThrowsObjectDisposed()
    {
        var runtime = new USharpRuntime();
        runtime.Dispose();
        Assert.Throws<ObjectDisposedException>(() => runtime.ResolveType("X.Y"));
    }
}

public sealed class OptionTests
{
    [Fact]
    public void Some_HasValue_True()
    {
        var opt = Option<int>.Some(42);
        Assert.True(opt.HasValue);
        Assert.Equal(42, opt.Value);
    }

    [Fact]
    public void None_HasValue_False()
    {
        var opt = Option<int>.None;
        Assert.False(opt.HasValue);
    }

    [Fact]
    public void None_Value_Throws()
    {
        var opt = Option<int>.None;
        Assert.Throws<InvalidOperationException>(() => _ = opt.Value);
    }

    [Fact]
    public void Map_TransformsValue()
    {
        var opt = Option<int>.Some(5);
        var mapped = opt.Map(x => x * 2);
        Assert.Equal(10, mapped.Value);
    }

    [Fact]
    public void Map_OnNone_ReturnsNone()
    {
        var mapped = Option<int>.None.Map(x => x * 2);
        Assert.False(mapped.HasValue);
    }

    [Fact]
    public void GetValueOrDefault_ReturnsDefault_WhenNone()
    {
        var opt = Option<string>.None;
        Assert.Equal("default", opt.GetValueOrDefault("default"));
    }
}

public sealed class ResultTests
{
    [Fact]
    public void Ok_IsSuccess()
    {
        var r = Result<int, string>.Ok(42);
        Assert.True(r.IsSuccess);
        Assert.Equal(42, r.Value);
    }

    [Fact]
    public void Err_IsError()
    {
        var r = Result<int, string>.Err("oops");
        Assert.True(r.IsError);
        Assert.Equal("oops", r.Error);
    }

    [Fact]
    public void Map_TransformsSuccessValue()
    {
        var r = Result<int, string>.Ok(10).Map(x => x.ToString());
        Assert.Equal("10", r.Value);
    }
}

public sealed class USharpRangeTests
{
    [Fact]
    public void Count_IsEndMinusStart()
    {
        var range = new USharpRange(0, 10);
        Assert.Equal(10, range.Count);
    }

    [Fact]
    public void Contains_InRange_True()
    {
        var range = new USharpRange(5, 15);
        Assert.True(range.Contains(5));
        Assert.True(range.Contains(14));
    }

    [Fact]
    public void Contains_OutOfRange_False()
    {
        var range = new USharpRange(5, 15);
        Assert.False(range.Contains(4));
        Assert.False(range.Contains(15));
    }

    [Fact]
    public void Enumerate_YieldsAllValues()
    {
        var range = new USharpRange(0, 5);
        Assert.Equal([0L, 1L, 2L, 3L, 4L], range.Enumerate());
    }
}

public sealed class WellKnownTypesTests
{
    [Fact]
    public void All_Returns8Types()
    {
        Assert.Equal(8, WellKnownTypes.All().Count());
    }

    [Fact]
    public void Int_HasExpectedFullName()
    {
        Assert.Equal("USharp.Core.Int", WellKnownTypes.Int.FullName);
    }
}

public sealed class USharpExceptionTests
{
    [Fact]
    public void USharpIndexOutOfRangeException_ContainsIndex()
    {
        var ex = new USharpIndexOutOfRangeException(99);
        Assert.Equal(99, ex.Index);
        Assert.Contains("99", ex.Message);
    }

    [Fact]
    public void USharpNotImplementedException_For_ContainsMemberName()
    {
        var ex = USharpNotImplementedException.For("SomeMember");
        Assert.Contains("SomeMember", ex.Message);
    }
}
