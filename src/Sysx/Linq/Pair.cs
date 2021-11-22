using System;
using System.ComponentModel;
using HashCode = Sysx.Hashing.HashCode;

namespace Sysx.Linq
{
    public class Pair<T> : Pair<T, T>
    {
        public Pair(T left, T right) : base(left, right) { }
    }

    public class Pair<TLeft, TRight>
    {
        public TLeft Left { get; }
        public TRight Right { get; }

        public Pair(TLeft left, TRight right)
        {
            Left = left;
            Right = right;
        }

        public override bool Equals(Object? obj)
        {
            if ((obj == null) || !GetType().Equals(obj.GetType())) return false;
            if (Left == null || Right == null) return false;
            var o = (Pair<TLeft, TRight>)obj;
            if (o.Left == null || o.Right == null) return false;
            return Left.Equals(o.Left) && Right.Equals(o.Right);
        }

        public static bool operator ==(Pair<TLeft, TRight> left, Pair<TLeft, TRight> right) => left.Equals(right);
        public static bool operator !=(Pair<TLeft, TRight> left, Pair<TLeft, TRight> right) => !(left == right);

        public override int GetHashCode() => HashCode.Combine(Left, Right);

        public override string ToString() => String.Format("Pair({0}, {1})", Left, Right);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out TLeft left, out TRight right)
        {
            left = Left;
            right = Right;
        }
    }
}
