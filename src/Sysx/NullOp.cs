namespace Sysx;

/// <summary>
/// Represents a potentially <see langword="null"/>able unknown value type
/// </summary>
internal interface INullOp<T>
{
    bool HasValue(T value);
    bool AddIfNotNull(ref T accumulator, T value);
}

/// <inheritdoc cref="INullOp{T}"/>
internal sealed class StructNullOp<T> : INullOp<T>, INullOp<T?> where T : struct
{
    /// <summary>
    /// Returns <see langword="true"/> if the <paramref name="value"/> is not <see langword="null"/>.
    /// </summary>
    public bool HasValue(T value) => true;

    /// <summary>
    /// Returns <see langword="true"/> if the <paramref name="value"/> is not <see langword="null"/>.
    /// </summary>
    public bool HasValue(T? value) => value.HasValue;

    /// <summary>
    /// Adds the <paramref name="value"/> to the <paramref name="accumulator"/> and returns <see langword="true"/> if the value is not <see langword="null"/>.
    /// </summary>
    public bool AddIfNotNull(ref T accumulator, T value)
    {
        accumulator = Operator<T>.Add(accumulator, value);
        return true;
    }

    /// <summary>
    /// Adds the <paramref name="value"/> to the <paramref name="accumulator"/> and returns <see langword="true"/> if the value is not <see langword="null"/>.
    /// </summary>
    public bool AddIfNotNull(ref T? accumulator, T? value)
    {
        if (value.HasValue)
        {
            accumulator = accumulator.HasValue
                ? Operator<T>.Add(accumulator.GetValueOrDefault(), value.GetValueOrDefault())
                : value;

            return true;
        }

        return false;
    }
}

/// <inheritdoc cref="INullOp{T}"/>
internal sealed class ClassNullOp<T> : INullOp<T> where T : class
{
    /// <summary>
    /// Returns <see langword="true"/> if the <paramref name="value"/> is not <see langword="null"/>.
    /// </summary>
    public bool HasValue(T value) =>  value != null;

    /// <summary>
    /// Adds the <paramref name="value"/> to the <paramref name="accumulator"/> and returns <see langword="true"/> if the value is not <see langword="null"/>.
    /// </summary>
    public bool AddIfNotNull(ref T accumulator, T value)
    {
        if (value != null)
        {
            accumulator = accumulator == null
                ? value
                : Operator<T>.Add(accumulator, value);

            return true;
        }

        return false;
    }
}