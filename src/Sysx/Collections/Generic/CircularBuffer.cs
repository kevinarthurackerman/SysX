namespace Sysx.Collections.Generic;

public static class ICircularBufferExtensions
{
    /// <summary>
    /// Element at the front of the buffer.
    /// </summary>
    public static bool TryPeekFirst<T>(this ICircularBuffer<T> circularBuffer, out T? element)
    {
        EnsureArg.IsNotNull(circularBuffer, nameof(circularBuffer));

        if (circularBuffer.Count == 0)
        {
            element = default;
            return false;
        }

        element = circularBuffer.PeekFirst();
        return true;
    }

    /// <summary>
    /// Element at the back of the buffer.
    /// </summary>
    public static bool TryPeekLast<T>(this ICircularBuffer<T> circularBuffer, out T? element)
    {
        EnsureArg.IsNotNull(circularBuffer, nameof(circularBuffer));

        if (circularBuffer.Count == 0)
        {
            element = default;
            return false;
        }

        element = circularBuffer.PeekLast();
        return true;
    }

    /// <summary>
    /// Pushes a new elements to the front of the buffer.
    /// </summary>
    public static void PushFirst<T>(this ICircularBuffer<T> circularBuffer, IEnumerable<T> elements)
    {
        EnsureArg.IsNotNull(circularBuffer, nameof(circularBuffer));
        EnsureArg.IsNotNull(elements, nameof(elements));

        foreach (var element in elements.Reverse())
            circularBuffer.PushFirst(element);
    }

    /// <summary>
    /// Pushes a new element to the end of the buffer.
    /// </summary>
    public static void PushLast<T>(this ICircularBuffer<T> circularBuffer, IEnumerable<T> elements)
    {
        EnsureArg.IsNotNull(circularBuffer, nameof(circularBuffer));
        EnsureArg.IsNotNull(elements, nameof(elements));

        foreach (var element in elements)
            circularBuffer.PushLast(element);
    }

    /// <summary>
    /// Removes the first elements in the buffer and returns them in their original order.
    /// </summary>
    public static IEnumerable<T> PopFirst<T>(this ICircularBuffer<T> circularBuffer, int count)
    {
        EnsureArg.IsNotNull(circularBuffer, nameof(circularBuffer));
        EnsureArg.IsGte(count, 0, nameof(count));

        var result = new T[count];

        for (var i = count - 1; i >= 0; i--)
            result[i] = circularBuffer.PopFirst();

        return result;
    }

    /// <summary>
    /// Removes the last elements in the buffer and returns them in their original order.
    /// </summary>
    public static IEnumerable<T> PopLast<T>(this ICircularBuffer<T> circularBuffer, int count)
    {
        EnsureArg.IsNotNull(circularBuffer, nameof(circularBuffer));
        EnsureArg.IsGte(count, 0, nameof(count));

        var result = new T[count];

        for (var i = count - 1; i >= 0; i--)
            result[i] = circularBuffer.PopLast();

        return result;
    }
}

/// <summary>
/// Collection that efficiently supports adding and removing elements to the beginning and end.
/// </summary>
public interface ICircularBuffer<T>
{
    /// <summary>
    /// The number of elements in the buffer.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Element at the front of the buffer.
    /// </summary>
    public T PeekFirst();

    /// <summary>
    /// Element at the back of the buffer.
    /// </summary>
    public T PeekLast();

    /// <summary>
    /// Pushes a new element to the front of the buffer.
    /// </summary>
    public void PushFirst(T item);

    /// <summary>
    /// Pushes a new element to the end of the buffer.
    /// </summary>
    public void PushLast(T item);

    /// <summary>
    /// Removes the first element in the buffer.
    /// </summary>
    public T PopFirst();

    /// <summary>
    /// Removes the last element in the buffer.
    /// </summary>
    public T PopLast();
}

/// <summary>
/// Collection that efficiently supports adding and removing elements to the beginning and end.
/// </summary>
public class CircularBuffer<T> : ICircularBuffer<T>, IEnumerable<T>, IEnumerable, ICollection, IReadOnlyCollection<T>
{
    private readonly Lazy<object> lazySyncRoot = new(() => new object());
    private T?[] buffer;
    private int start;
    private int end;

    public CircularBuffer() : this(4, OverflowBehaviors.Expand, Array.Empty<T>()) { }

    public CircularBuffer(int capacity, OverflowBehaviors overflowBehavior)
        : this(capacity, overflowBehavior, Array.Empty<T>()) { }

    public CircularBuffer(int capacity, OverflowBehaviors overflowBehavior, T[] items)
    {
        EnsureArg.IsGt(capacity, 0, nameof(capacity));
        EnsureArg.EnumIsDefined(overflowBehavior, nameof(overflowBehavior));
        EnsureArg.IsNotNull(items, nameof(items));
        EnsureArg.IsTrue(items.Length <= capacity, optsFn: x => x.WithMessage("Item count must not exceed capacity."));

        buffer = new T[capacity];

        Array.Copy(items, buffer, items.Length);
        Count = items.Length;

        start = 0;
        end = Count == capacity ? 0 : Count;
        
        OverflowBehavior = overflowBehavior;
    }

    /// <summary>
    /// Value indicating if the buffer is thread safe. Always returns false.
    /// </summary>
    bool ICollection.IsSynchronized => false;

    /// <summary>
    /// Gets an object that can be used to synchronize access.
    /// </summary>
    object ICollection.SyncRoot => lazySyncRoot.Value;

    /// <summary>
    /// Behavior that triggers when an element is added to the buffer when it is already at capacity.
    /// </summary>
    public OverflowBehaviors OverflowBehavior { get; }

    /// <summary>
    /// The number of elements in the buffer.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Capacity of the buffer. Elements added to the buffer after this limit will trigger the overflow behavior.
    /// </summary>
    public int Capacity => buffer.Length;

    /// <summary>
    /// Indicates if the buffer is full. Elements added to the buffer after this limit will trigger the overflow behavior.
    /// </summary>
    public bool IsFull => Count == Capacity;

    /// <summary>
    /// True if has no elements.
    /// </summary>
    public bool IsEmpty => Count == 0;

    /// <summary>
    /// Element at the front of the buffer - this[0].
    /// </summary>
    public T PeekFirst()
    {
        if (IsEmpty) throw new InvalidOperationException("Cannot access an empty buffer.");
        return buffer[start]!;
    }

    /// <summary>
    /// Element at the back of the buffer - this[^1].
    /// </summary>
    public T PeekLast()
    {
        if (IsEmpty) throw new InvalidOperationException("Cannot access an empty buffer.");
        return buffer[(end != 0 ? end : Capacity) - 1]!;
    }

    /// <summary>
    /// Index access to elements in buffer.
    /// Index does not loop around like when adding elements.
    /// </summary>
    public T this[int index]
    {
        get
        {
            EnsureArg.IsFalse(IsEmpty, optsFn: x => x.WithException(new IndexOutOfRangeException($"Cannot access index {index}. Buffer is empty")));
            EnsureArg.IsFalse(index >= Count, optsFn: x => x.WithException(new IndexOutOfRangeException($"Cannot access index {index}. Buffer size is {Count}")));

            return buffer[ConvertIndex(index)]!;
        }
        set
        {
            EnsureArg.IsFalse(IsEmpty, optsFn: x => x.WithException(new IndexOutOfRangeException($"Cannot access index {index}. Buffer is empty")));
            EnsureArg.IsFalse(index >= Count, optsFn: x => x.WithException(new IndexOutOfRangeException($"Cannot access index {index}. Buffer size is {Count}")));

            buffer[ConvertIndex(index)] = value;
        }
    }

    /// <summary>
    /// Pushes a new element to the back of the buffer. PeekLast()/this[^1] will now return this element.
    /// </summary>
    public void PushLast(T item)
    {
        if (IsFull)
        {
            switch (OverflowBehavior)
            {
                case OverflowBehaviors.Expand:
                    ExpandCapacity();
                    buffer[end] = item;
                    Increment(ref end);
                    ++Count;
                    break;
                case OverflowBehaviors.Overwrite:
                    buffer[end] = item;
                    Increment(ref end);
                    start = end;
                    break;
                case OverflowBehaviors.ThrowException:
                    throw new OverflowException("The buffer is full. Adding an element would exceed the capacity.");
            }
        }
        else
        {
            buffer[end] = item;
            Increment(ref end);
            ++Count;
        }
    }

    /// <summary>
    /// Pushes a new element to the front of the buffer. PeekFirst()/this[0] will now return this element.
    /// </summary>
    public void PushFirst(T item)
    {
        if (IsFull)
        {
            switch (OverflowBehavior)
            {
                case OverflowBehaviors.Expand:
                    ExpandCapacity();
                    Decrement(ref start);
                    buffer[start] = item;
                    ++Count;
                    break;
                case OverflowBehaviors.Overwrite:
                    Decrement(ref start);
                    end = start;
                    buffer[start] = item;
                    break;
                case OverflowBehaviors.ThrowException:
                    throw new OverflowException("The buffer is full. Adding an element would exceed the capacity.");
            }
        }
        else
        {
            Decrement(ref start);
            buffer[start] = item;
            ++Count;
        }
    }

    /// <summary>
    /// Removes the last element of the buffer.
    /// </summary>
    public T PopLast()
    {
        if (IsEmpty) throw new InvalidOperationException("Cannot take elements from an empty buffer.");
        Decrement(ref end);

        var result = buffer[end];

        buffer[end] = default;
        --Count;

        return result!;
    }

    /// <summary>
    /// Removes the first element in the buffer.
    /// </summary>
    public T PopFirst()
    {
        if (IsEmpty) throw new InvalidOperationException("Cannot take elements from an empty buffer.");

        var result = buffer[start];

        buffer[start] = default;
        Increment(ref start);
        --Count;

        return result!;
    }

    /// <summary>
    /// Copies the buffer contents to an array, according to the logical
    /// contents of the buffer (i.e. independent of the internal order/contents)
    /// </summary>
    public T[] ToArray()
    {
        T[] newArray = new T[Count];

        CopyTo(newArray, 0);

        return newArray;
    }

    /// <summary>
    /// Get the contents of the buffer as 2 ArraySegments. Respects the logical contents of the buffer, where
    /// each segment and items in each segment are ordered according to insertion.
    ///
    /// <remarks>Segments may be empty.</remarks>
    /// </summary>
    public ArraySegments ToArraySegments()
    {
        if (IsEmpty)
        {
            return new ArraySegments(
                new ArraySegment<T>(Array.Empty<T>()),
                new ArraySegment<T>(Array.Empty<T>()));
        }
        else if (start < end)
        {
            return new ArraySegments(
                new ArraySegment<T>(buffer!, start, end - start),
                new ArraySegment<T>(buffer!, end, 0));
        }
        else
        {
            return new ArraySegments(
                new ArraySegment<T>(buffer!, start, buffer.Length - start),
                new ArraySegment<T>(buffer!, 0, end));
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the buffer.
    /// </summary>
    public IEnumerator<T> GetEnumerator()
    {
        var (first, second) = ToArraySegments();

        for (int i = 0; i < first.Count; i++)
            yield return first.Array![first.Offset + i];

        for (int i = 0; i < second.Count; i++)
            yield return second.Array![second.Offset + i];
    }

    /// <summary>
    /// Returns an enumerator that iterates through the buffer.
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void CopyTo(Array array, int index)
    {
        EnsureArg.IsNotNull(array, nameof(array));
        EnsureArg.IsGte(index, 0, optsFn: x => x.WithException(new ArgumentOutOfRangeException(nameof(index), "Index must be greater than or equal to 0.")));
        EnsureArg.IsGte(array.Length, index + Count, optsFn: x => x.WithException(new ArgumentOutOfRangeException(nameof(index), "Length of array must be greater than or equal to the start index plus number of elements to be copied.")));

        var (first, second) = ToArraySegments();

        Array.Copy(first.Array!, first.Offset, array, index, first.Count);
        Array.Copy(second.Array!, second.Offset, array, first.Count + index, second.Count);
    }

    private void Increment(ref int index)
    {
        if (++index == Capacity) index = 0;
    }

    private void Decrement(ref int index)
    {
        if (index == 0) index = Capacity;
        
        index--;
    }

    private int ConvertIndex(int index) =>
        start + (index < (Capacity - start) ? index : index - Capacity);

    private void ExpandCapacity()
    {
        var newBuffer = new T?[buffer.Length * 2];

        if (start < end)
        {
            Array.Copy(buffer, start, newBuffer, 0, end - start);
        }
        else
        {
            Array.Copy(buffer, start, newBuffer, 0, Count - start);
            Array.Copy(buffer, 0, newBuffer, Count - start, end);
        }

        start = 0;
        end = Count;
        buffer = newBuffer;
    }

    public readonly record struct ArraySegments(ArraySegment<T> First, ArraySegment<T> Second);

    public enum OverflowBehaviors
    {
        Expand = 0,
        Overwrite = 1,
        ThrowException = 2
    }
}