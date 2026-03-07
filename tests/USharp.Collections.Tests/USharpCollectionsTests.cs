using USharp.Collections;
using Xunit;

namespace USharp.Collections.Tests;

public sealed class USharpListTests
{
    [Fact]
    public void Add_IncreasesCount()
    {
        var list = new USharpList<int>();
        list.Add(1);
        list.Add(2);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void Filter_ReturnsMatchingElements()
    {
        var list = new USharpList<int>([1, 2, 3, 4, 5]);
        var evens = list.Filter(x => x % 2 == 0);
        Assert.Equal([2, 4], evens);
    }

    [Fact]
    public void Map_TransformsElements()
    {
        var list = new USharpList<int>([1, 2, 3]);
        var doubled = list.Map(x => x * 2);
        Assert.Equal([2, 4, 6], doubled);
    }

    [Fact]
    public void Reduce_SumsElements()
    {
        var list = new USharpList<int>([1, 2, 3, 4]);
        var sum = list.Reduce(0, (acc, x) => acc + x);
        Assert.Equal(10, sum);
    }

    [Fact]
    public void Slice_ReturnsSubList()
    {
        var list = new USharpList<int>([10, 20, 30, 40, 50]);
        var slice = list.Slice(1, 3);
        Assert.Equal([20, 30, 40], slice);
    }

    [Fact]
    public void Sort_OrdersElements()
    {
        var list = new USharpList<int>([5, 1, 3, 2, 4]);
        list.Sort();
        Assert.Equal([1, 2, 3, 4, 5], list);
    }

    [Fact]
    public void FirstOrDefault_EmptyList_ReturnsDefault()
    {
        var list = new USharpList<string>();
        Assert.Null(list.FirstOrDefault());
    }

    [Fact]
    public void LastOrDefault_ReturnsLastElement()
    {
        var list = new USharpList<int>([10, 20, 30]);
        Assert.Equal(30, list.LastOrDefault());
    }
}

public sealed class USharpMapTests
{
    [Fact]
    public void Add_ContainsKey()
    {
        var map = new USharpMap<string, int>();
        map.Add("a", 1);
        Assert.True(map.ContainsKey("a"));
    }

    [Fact]
    public void GetOrDefault_MissingKey_ReturnsDefault()
    {
        var map = new USharpMap<string, int>();
        Assert.Equal(-1, map.GetOrDefault("missing", -1));
    }

    [Fact]
    public void GetOrAdd_AddsWhenMissing()
    {
        var map = new USharpMap<string, int>();
        var value = map.GetOrAdd("key", _ => 42);
        Assert.Equal(42, value);
        Assert.True(map.ContainsKey("key"));
    }

    [Fact]
    public void Filter_ReturnsMatchingPairs()
    {
        var map = new USharpMap<string, int>(new Dictionary<string, int>
        {
            { "a", 1 }, { "b", 2 }, { "c", 3 }
        });
        var filtered = map.Filter((_, v) => v > 1);
        Assert.Equal(2, filtered.Count);
        Assert.False(filtered.ContainsKey("a"));
    }

    [Fact]
    public void Merge_OverwritesExistingKeys()
    {
        var map = new USharpMap<string, int>();
        map.Add("x", 1);
        var other = new USharpMap<string, int>();
        other.Add("x", 99);
        map.Merge(other);
        Assert.Equal(99, map["x"]);
    }
}

public sealed class USharpStackTests
{
    [Fact]
    public void PushPop_LIFO()
    {
        var stack = new USharpStack<int>();
        stack.Push(1);
        stack.Push(2);
        stack.Push(3);
        Assert.Equal(3, stack.Pop());
        Assert.Equal(2, stack.Pop());
    }

    [Fact]
    public void IsEmpty_WhenEmpty_True()
    {
        var stack = new USharpStack<string>();
        Assert.True(stack.IsEmpty);
    }

    [Fact]
    public void TryPop_WhenEmpty_ReturnsFalse()
    {
        var stack = new USharpStack<int>();
        Assert.False(stack.TryPop(out _));
    }
}

public sealed class USharpQueueTests
{
    [Fact]
    public void EnqueueDequeue_FIFO()
    {
        var q = new USharpQueue<int>();
        q.Enqueue(1);
        q.Enqueue(2);
        q.Enqueue(3);
        Assert.Equal(1, q.Dequeue());
        Assert.Equal(2, q.Dequeue());
    }
}

public sealed class USharpSetTests
{
    [Fact]
    public void Add_NoDuplicates()
    {
        var set = new USharpSet<int>();
        set.Add(1);
        set.Add(1);
        set.Add(2);
        Assert.Equal(2, set.Count);
    }

    [Fact]
    public void Union_CombinesTwoSets()
    {
        var a = new USharpSet<int>([1, 2, 3]);
        var b = new USharpSet<int>([3, 4, 5]);
        var union = a.Union(b);
        Assert.Equal(5, union.Count);
    }

    [Fact]
    public void Intersect_ReturnsCommonElements()
    {
        var a = new USharpSet<int>([1, 2, 3]);
        var b = new USharpSet<int>([2, 3, 4]);
        var intersection = a.Intersect(b);
        Assert.Equal(2, intersection.Count);
        Assert.Contains(2, intersection);
        Assert.Contains(3, intersection);
    }

    [Fact]
    public void Except_RemovesElementsFromOther()
    {
        var a = new USharpSet<int>([1, 2, 3]);
        var b = new USharpSet<int>([2, 3]);
        var diff = a.Except(b);
        Assert.Single(diff);
        Assert.Contains(1, diff);
    }
}
