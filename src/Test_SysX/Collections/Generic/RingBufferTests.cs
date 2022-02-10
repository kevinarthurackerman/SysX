namespace Test_SysX.Collections.Generic;
using Assert = Xunit.Assert;

public class RingBufferTests
{
    [Fact]
    public void Should_Be_Empty()
    {
        var buffer = new CircularBuffer<int>();
        
        Assert.True(buffer.IsEmpty);
        Assert.Empty(buffer.ToArray());
        Assert.NotNull(Record.Exception(() => buffer[0]));
    }

    [Fact]
    public void Should_Construct_With_Elements()
    {
        var buffer = new CircularBuffer<int>(4, CircularBuffer<int>.OverflowBehaviors.Expand, new[] { 4, 3, 2, 1 });

        Assert.Equal(4, buffer[0]);
        Assert.Equal(3, buffer[1]);
        Assert.Equal(2, buffer[2]);
        Assert.Equal(1, buffer[3]);
    }

    [Fact]
    public void Should_Get_First_And_Last()
    {
        var buffer = new CircularBuffer<int>();
        buffer.PushLast(1);
        buffer.PushLast(2);
        buffer.PushLast(3);

        Assert.Equal(1, buffer.PeekFirst());
        Assert.Equal(3, buffer.PeekLast());
    }

    [Fact]
    public void Should_Not_Construct_When_Exceeds_Capacity()
    {
        Assert.NotNull(Record.Exception(() => new CircularBuffer<int>(4, CircularBuffer<int>.OverflowBehaviors.Expand, new[] { 5, 4, 3, 2, 1 })));

    }

    [Fact]
    public void Should_Overwrite()
    {
        var buffer = new CircularBuffer<int>(4, CircularBuffer<int>.OverflowBehaviors.Overwrite);

        Assert.Equal(4, buffer.Capacity);

        buffer.PushLast(1);
        buffer.PushLast(2);
        buffer.PushLast(3);
        buffer.PushLast(4);
        buffer.PushLast(5);
        buffer.PushLast(6);

        Assert.Equal(4, buffer.Capacity);

        Assert.Equal(3, buffer[0]);
        Assert.Equal(4, buffer[1]);
        Assert.Equal(5, buffer[2]);
        Assert.Equal(6, buffer[3]);
    }

    [Fact]
    public void Should_Push()
    {
        var buffer = new CircularBuffer<int>();

        buffer.PushFirst(1);
        buffer.PushFirst(2);
        buffer.PushLast(3);
        buffer.PushLast(4);
        Assert.Equal(2, buffer[0]);
        Assert.Equal(1, buffer[1]);
        Assert.Equal(3, buffer[2]);
        Assert.Equal(4, buffer[3]);
    }

    [Fact]
    public void Should_Expand_Capacity()
    {
        var buffer = new CircularBuffer<int>();

        Assert.Equal(4, buffer.Capacity);

        buffer.PushFirst(1);
        buffer.PushFirst(2);
        buffer.PushFirst(3);
        buffer.PushFirst(4);
        buffer.PushFirst(5);

        Assert.Equal(8, buffer.Capacity);

        Assert.Equal(5, buffer[0]);
        Assert.Equal(4, buffer[1]);
        Assert.Equal(3, buffer[2]);
        Assert.Equal(2, buffer[3]);
        Assert.Equal(1, buffer[4]);
    }

    [Fact]
    public void Should_Not_Expand_Capacity()
    {
        var buffer = new CircularBuffer<int>();

        Assert.Equal(4, buffer.Capacity);

        buffer.PushFirst(1);
        buffer.PushFirst(2);
        buffer.PushFirst(3);
        buffer.PushFirst(4);
        buffer.PopFirst();
        buffer.PushFirst(5);

        Assert.Equal(4, buffer.Capacity);

        Assert.Equal(5, buffer[0]);
        Assert.Equal(3, buffer[1]);
        Assert.Equal(2, buffer[2]);
        Assert.Equal(1, buffer[3]);
    }

    [Fact]
    public void Should_Expand_Capacity_When_Looped()
    {
        var buffer = new CircularBuffer<int>();

        Assert.Equal(4, buffer.Capacity);

        buffer.PushLast(1);
        buffer.PushLast(2);
        buffer.PushLast(3);
        buffer.PushLast(4);
        buffer.PopFirst();
        buffer.PushLast(5);
        buffer.PushLast(6);

        Assert.Equal(8, buffer.Capacity);

        Assert.Equal(2, buffer[0]);
        Assert.Equal(3, buffer[1]);
        Assert.Equal(4, buffer[2]);
        Assert.Equal(5, buffer[3]);
        Assert.Equal(6, buffer[4]);
    }

    [Fact]
    public void Should_Not_Expand_Capacity_When_Looped()
    {
        var buffer = new CircularBuffer<int>();

        Assert.Equal(4, buffer.Capacity);

        buffer.PushLast(1);
        buffer.PushLast(2);
        buffer.PushLast(3);
        buffer.PushLast(4);
        buffer.PopFirst();
        buffer.PopFirst();
        buffer.PushLast(5);
        buffer.PushLast(6);

        Assert.Equal(4, buffer.Capacity);

        Assert.Equal(3, buffer[0]);
        Assert.Equal(4, buffer[1]);
        Assert.Equal(5, buffer[2]);
        Assert.Equal(6, buffer[3]);
    }


    [Fact]
    public void Should_Correct_Collection_Order()
    {
        var buffer = new CircularBuffer<int>();

        Assert.Equal(4, buffer.Capacity);

        buffer.PushLast(1);
        buffer.PushLast(2);
        buffer.PushLast(3);
        buffer.PushLast(4);
        buffer.PopFirst();
        buffer.PopFirst();
        buffer.PushLast(5);
        buffer.PushLast(6);

        Assert.Equal(new[] { 3, 4, 5, 6 }, buffer);
        Assert.Equal(new[] { 3, 4, 5, 6 }, buffer.ToArray());
        Assert.Equal(new[] { 3, 4 }, buffer.ToArraySegments().First);
        Assert.Equal(new[] { 5, 6 }, buffer.ToArraySegments().Second);

        var arr = new int[7];
        buffer.CopyTo(arr, 2);
        Assert.Equal(new [] { 0, 0, 3, 4, 5, 6, 0 }, arr);
    }
}
