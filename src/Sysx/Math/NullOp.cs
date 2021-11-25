namespace Sysx.Math
{
    /// <summary>
    /// Represents a potentially nullable unknown value type
    /// </summary>
    internal interface INullOp<T>
    {
        bool HasValue(T value);
        bool AddIfNotNull(ref T accumulator, T value);
    }

    /// <inheritdoc cref="INullOp{T}"/>
    internal sealed class StructNullOp<T> : INullOp<T>, INullOp<T?> where T : struct
    {
        public bool HasValue(T value) => true;
        
        public bool AddIfNotNull(ref T accumulator, T value)
        {
            accumulator = Operator<T>.Add(accumulator, value);
            return true;
        }

        public bool HasValue(T? value) => value.HasValue;
        
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
        public bool HasValue(T value) =>  value != null;
        
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
}
