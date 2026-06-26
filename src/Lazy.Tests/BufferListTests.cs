namespace Lazy.Tests;

using System;
using Lazy.Buffer;
using Xunit;

public class BufferListTests
{
    [Fact]
    public void Constructor_InvalidBufferLengths_ThrowsArgumentException()
    {
        bool threw = false;
        try
        {
            Span<int> buffer = stackalloc int[5];
            Span<bool> validation = stackalloc bool[4];
            var list = new BufferList<int>(buffer, validation);
        }
        catch (ArgumentException)
        {
            threw = true;
        }
        Assert.True(threw);
    }

    [Fact]
    public void Constructor_ValidLengths_InitializesCorrectly()
    {
        Span<int> buffer = stackalloc int[5];
        Span<bool> validation = stackalloc bool[5];
        var list = new BufferList<int>(buffer, validation);

        Assert.Equal(0, list.Count);
        Assert.Equal(5, list.Capacity);
        Assert.True(list.IsEmpty);
        Assert.False(list.IsFull);
    }

    [Fact]
    public void Append_WhenEmpty_AddsElementAndIncrementsCount()
    {
        Span<int> buffer = stackalloc int[3];
        Span<bool> validation = stackalloc bool[3];
        var list = new BufferList<int>(buffer, validation);

        list.Append(10);
        Assert.Equal(1, list.Count);
        Assert.Equal(10, list.PeekFirst());
        Assert.Equal(10, list.PeekLast());
    }

    [Fact]
    public void Append_WhenFull_OverwritesOldestElement_CircularLogic()
    {
        Span<int> buffer = stackalloc int[3];
        Span<bool> validation = stackalloc bool[3];
        var list = new BufferList<int>(buffer, validation);

        list.Append(1);
        list.Append(2);
        list.Append(3);

        Assert.True(list.IsFull);
        Assert.Equal(1, list.PeekFirst());
        Assert.Equal(3, list.PeekLast());

        list.Append(4); // Overwrites 1
        Assert.True(list.IsFull);
        Assert.Equal(3, list.Count);
        Assert.Equal(2, list.PeekFirst());
        Assert.Equal(4, list.PeekLast());
    }

    [Fact]
    public void RemoveFirst_ReturnsAndRemovesOldestElement()
    {
        Span<int> buffer = stackalloc int[3];
        Span<bool> validation = stackalloc bool[3];
        var list = new BufferList<int>(buffer, validation);

        list.Append(1);
        list.Append(2);

        int removed = list.RemoveFirst();
        Assert.Equal(1, removed);
        Assert.Equal(1, list.Count);
        Assert.Equal(2, list.PeekFirst());
    }

    [Fact]
    public void RemoveFirst_WhenEmpty_ThrowsInvalidOperationException()
    {
        bool threw = false;
        try
        {
            Span<int> buffer = stackalloc int[3];
            Span<bool> validation = stackalloc bool[3];
            var list = new BufferList<int>(buffer, validation);
            list.RemoveFirst();
        }
        catch (InvalidOperationException)
        {
            threw = true;
        }
        Assert.True(threw);
    }

    [Fact]
    public void TryFind_ExistingElement_ReturnsTrueAndElement()
    {
        Span<int> buffer = stackalloc int[5];
        Span<bool> validation = stackalloc bool[5];
        var list = new BufferList<int>(buffer, validation);

        list.Append(10);
        list.Append(20);
        list.Append(30);

        bool found = list.TryFind(x => x == 20, out int result);
        Assert.True(found);
        Assert.Equal(20, result);
    }

    [Fact]
    public void TryFind_NonExistingElement_ReturnsFalse()
    {
        Span<int> buffer = stackalloc int[5];
        Span<bool> validation = stackalloc bool[5];
        var list = new BufferList<int>(buffer, validation);

        list.Append(10);
        list.Append(20);

        bool found = list.TryFind(x => x == 30, out int result);
        Assert.False(found);
        Assert.Equal(default, result);
    }

    [Fact]
    public void TryPeekFirst_WhenNotEmpty_ReturnsTrueAndFirstElement()
    {
        Span<int> buffer = stackalloc int[3];
        Span<bool> validation = stackalloc bool[3];
        var list = new BufferList<int>(buffer, validation);

        list.Append(1);
        list.Append(2);

        bool success = list.TryPeekFirst(out int result);
        Assert.True(success);
        Assert.Equal(1, result);
    }

    [Fact]
    public void TryPeekFirst_WhenEmpty_ReturnsFalse()
    {
        Span<int> buffer = stackalloc int[3];
        Span<bool> validation = stackalloc bool[3];
        var list = new BufferList<int>(buffer, validation);

        bool success = list.TryPeekFirst(out int result);
        Assert.False(success);
        Assert.Equal(default, result);
    }

    [Fact]
    public void TryPeekLast_WhenNotEmpty_ReturnsTrueAndLastElement()
    {
        Span<int> buffer = stackalloc int[3];
        Span<bool> validation = stackalloc bool[3];
        var list = new BufferList<int>(buffer, validation);

        list.Append(1);
        list.Append(2);

        bool success = list.TryPeekLast(out int result);
        Assert.True(success);
        Assert.Equal(2, result);
    }

    [Fact]
    public void PeekFirstValid_WithGaps_FindsFirstValid()
    {
        Span<int> buffer = stackalloc int[5];
        Span<bool> validation = stackalloc bool[5];
        var list = new BufferList<int>(buffer, validation);

        list.Append(1);
        list.Append(2);
        list.Append(3);
        list.Release(0); // Removes 1, index 0 is now gap

        int firstValid = list.PeekFirstValid();
        Assert.Equal(2, firstValid);
    }

    [Fact]
    public void PeekLastValid_WithGaps_FindsLastValid()
    {
        Span<int> buffer = stackalloc int[5];
        Span<bool> validation = stackalloc bool[5];
        var list = new BufferList<int>(buffer, validation);

        list.Append(1);
        list.Append(2);
        list.Append(3);
        list.Release(2); // Removes 3, index 2 is now gap

        int lastValid = list.PeekLastValid();
        Assert.Equal(2, lastValid);
    }

    [Fact]
    public void Insert_IntoEmptySlot_AddsElement()
    {
        Span<int> buffer = stackalloc int[5];
        Span<bool> validation = stackalloc bool[5];
        var list = new BufferList<int>(buffer, validation);

        list.Append(1);
        list.Append(2);
        list.Release(0); // Free up slot 0

        list.Insert(99); // Should insert at slot 0
        Assert.Equal(2, list.Count);
        Assert.Equal(99, list.PeekFirstValid());
    }

    [Fact]
    public void Insert_WhenFull_ThrowsOverflowException()
    {
        bool threw = false;
        try
        {
            Span<int> buffer = stackalloc int[2];
            Span<bool> validation = stackalloc bool[2];
            var list = new BufferList<int>(buffer, validation);

            list.Append(1);
            list.Append(2);
            list.Insert(3);
        }
        catch (OverflowException)
        {
            threw = true;
        }
        Assert.True(threw);
    }

    [Fact]
    public void AppendGrow_WhenNotFull_ActsLikeAppend()
    {
        Span<int> buffer = stackalloc int[5];
        Span<bool> validation = stackalloc bool[5];
        var list = new BufferList<int>(buffer, validation);

        Span<int> newBuffer = stackalloc int[10];
        Span<bool> newValidation = stackalloc bool[10];

        list.AppendGrow(1, newBuffer, newValidation);
        Assert.Equal(1, list.Count);
        Assert.Equal(1, list.PeekFirst());
        Assert.Equal(5, list.Capacity); // Doesn't grow because not full
    }

    [Fact]
    public void AppendGrow_WhenFull_ResizesAndAppends()
    {
        Span<int> buffer = stackalloc int[2];
        Span<bool> validation = stackalloc bool[2];
        var list = new BufferList<int>(buffer, validation);

        list.Append(1);
        list.Append(2);

        Span<int> newBuffer = stackalloc int[5];
        Span<bool> newValidation = stackalloc bool[5];

        list.AppendGrow(3, newBuffer, newValidation);
        Assert.Equal(3, list.Count);
        Assert.Equal(5, list.Capacity);
        Assert.Equal(1, list.PeekFirst());
        Assert.Equal(3, list.PeekLast());
    }

    [Fact]
    public void Update_ValidIndex_UpdatesElement()
    {
        Span<int> buffer = stackalloc int[3];
        Span<bool> validation = stackalloc bool[3];
        var list = new BufferList<int>(buffer, validation);

        list.Append(10);
        list.Append(20);
        list.Append(30);

        list.Update(1, 99);
        Assert.Equal(99, list.ToArray()[1]);
    }

    [Fact]
    public void UpdateWhere_MatchingCondition_UpdatesElements()
    {
        Span<int> buffer = stackalloc int[5];
        Span<bool> validation = stackalloc bool[5];
        var list = new BufferList<int>(buffer, validation);

        list.Append(1);
        list.Append(2);
        list.Append(3);
        list.Append(4);

        list.UpdateWhere(x => x % 2 == 0, 0); // Update even to 0

        var array = list.ToArray();
        Assert.Equal(1, array[0]);
        Assert.Equal(0, array[1]);
        Assert.Equal(3, array[2]);
        Assert.Equal(0, array[3]);
    }

    [Fact]
    public void Remove_LogicalIndex_RemovesAndOrganizes()
    {
        Span<int> buffer = stackalloc int[5];
        Span<bool> validation = stackalloc bool[5];
        var list = new BufferList<int>(buffer, validation);

        list.Append(10);
        list.Append(20);
        list.Append(30);

        list.Remove(1); // removes 20, shifts 30
        Assert.Equal(2, list.Count);
        var array = list.ToArray();
        Assert.Equal(10, array[0]);
        Assert.Equal(30, array[1]);
    }

    [Fact]
    public void Remove_LogicalIndex_WithPurge_RemovesAndPurges()
    {
        Span<int> buffer = stackalloc int[5];
        Span<bool> validation = stackalloc bool[5];
        var list = new BufferList<int>(buffer, validation);

        list.Append(10);
        list.Append(20);
        list.Append(30);

        list.Remove(1, purge: true);
        Assert.Equal(2, list.Count);
        Assert.Equal(2, list.Capacity); // Purge truncates capacity
        var array = list.ToArray();
        Assert.Equal(10, array[0]);
        Assert.Equal(30, array[1]);
    }

    [Fact]
    public void IndexOf_ExistingItem_ReturnsLogicalIndex()
    {
        Span<int> buffer = stackalloc int[5];
        Span<bool> validation = stackalloc bool[5];
        var list = new BufferList<int>(buffer, validation);

        list.Append(10);
        list.Append(20);
        list.Append(30);

        Assert.Equal(1, list.IndexOf(20));
        Assert.Equal(2, list.IndexOf(30));
        Assert.True(list.Contains(20));
    }

    [Fact]
    public void IndexOf_NonExistingItem_ReturnsMinusOne()
    {
        Span<int> buffer = stackalloc int[5];
        Span<bool> validation = stackalloc bool[5];
        var list = new BufferList<int>(buffer, validation);

        list.Append(10);
        list.Append(20);

        Assert.Equal(-1, list.IndexOf(30));
        Assert.False(list.Contains(30));
    }

    [Fact]
    public void Clear_EmptiesList()
    {
        Span<int> buffer = stackalloc int[5];
        Span<bool> validation = stackalloc bool[5];
        var list = new BufferList<int>(buffer, validation);

        list.Append(10);
        list.Append(20);

        list.Clear();
        Assert.Equal(0, list.Count);
        Assert.True(list.IsEmpty);
    }

    [Fact]
    public void Sort_OrdersElementsCorrectly()
    {
        Span<int> buffer = stackalloc int[5];
        Span<bool> validation = stackalloc bool[5];
        var list = new BufferList<int>(buffer, validation);

        list.Append(30);
        list.Append(10);
        list.Append(20);

        list.Sort((a, b) => a.CompareTo(b));

        var array = list.ToArray();
        Assert.Equal(10, array[0]);
        Assert.Equal(20, array[1]);
        Assert.Equal(30, array[2]);
    }

    [Fact]
    public void Organize_ShiftsValidItemsToFront()
    {
        Span<int> buffer = stackalloc int[5];
        Span<bool> validation = stackalloc bool[5];
        var list = new BufferList<int>(buffer, validation);

        list.Append(10);
        list.Append(20);
        list.Append(30);

        list.Release(1); // logical index 1, value 20 -> becomes gap

        list.Organize();
        var array = list.ToArray();
        Assert.Equal(2, list.Count);
        Assert.Equal(10, array[0]);
        Assert.Equal(30, array[1]);
    }

    [Fact]
    public void Truncate_ReducesCountAndRemovesOldest()
    {
        Span<int> buffer = stackalloc int[5];
        Span<bool> validation = stackalloc bool[5];
        var list = new BufferList<int>(buffer, validation);

        list.Append(10);
        list.Append(20);
        list.Append(30);
        list.Append(40);

        list.Truncate(2); // Should keep 30 and 40
        Assert.Equal(2, list.Count);
        Assert.Equal(30, list.PeekFirst());
        Assert.Equal(40, list.PeekLast());
    }

    [Fact]
    public void ToList_ReturnsGenericList()
    {
        Span<int> buffer = stackalloc int[5];
        Span<bool> validation = stackalloc bool[5];
        var list = new BufferList<int>(buffer, validation);

        list.Append(10);
        list.Append(20);

        var genericList = list.ToList();
        Assert.Equal(2, genericList.Count);
        Assert.Equal(10, genericList[0]);
        Assert.Equal(20, genericList[1]);
    }

    [Fact]
    public void Enumerator_IteratesOverValidItems()
    {
        Span<int> buffer = stackalloc int[5];
        Span<bool> validation = stackalloc bool[5];
        var list = new BufferList<int>(buffer, validation);

        list.Append(10);
        list.Append(20);
        list.Append(30);

        int sum = 0;
        foreach (var item in list)
        {
            sum += item;
        }

        Assert.Equal(60, sum);
    }
}