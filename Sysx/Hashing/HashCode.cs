namespace Sysx.Hashing
{
    public static class HashCode
    {
        public static int Combine<T1>(T1 item1)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + item1?.GetHashCode() ?? 0;
                return hash;
            }
        }

        public static int Combine<T1, T2>(T1 item1, T2 item2)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + item1?.GetHashCode() ?? 0;
                hash = hash * 31 + item2?.GetHashCode() ?? 0;
                return hash;
            }
        }

        public static int Combine<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + item1?.GetHashCode() ?? 0;
                hash = hash * 31 + item2?.GetHashCode() ?? 0;
                hash = hash * 31 + item3?.GetHashCode() ?? 0;
                return hash;
            }
        }

        public static int Combine<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + item1?.GetHashCode() ?? 0;
                hash = hash * 31 + item2?.GetHashCode() ?? 0;
                hash = hash * 31 + item3?.GetHashCode() ?? 0;
                hash = hash * 31 + item4?.GetHashCode() ?? 0;
                return hash;
            }
        }

        public static int Combine<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + item1?.GetHashCode() ?? 0;
                hash = hash * 31 + item2?.GetHashCode() ?? 0;
                hash = hash * 31 + item3?.GetHashCode() ?? 0;
                hash = hash * 31 + item4?.GetHashCode() ?? 0;
                hash = hash * 31 + item5?.GetHashCode() ?? 0;
                return hash;
            }
        }

        public static int Combine<T1, T2, T3, T4, T5, T6>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + item1?.GetHashCode() ?? 0;
                hash = hash * 31 + item2?.GetHashCode() ?? 0;
                hash = hash * 31 + item3?.GetHashCode() ?? 0;
                hash = hash * 31 + item4?.GetHashCode() ?? 0;
                hash = hash * 31 + item5?.GetHashCode() ?? 0;
                hash = hash * 31 + item6?.GetHashCode() ?? 0;
                return hash;
            }
        }

        public static int Combine<T1, T2, T3, T4, T5, T6, T7>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + item1?.GetHashCode() ?? 0;
                hash = hash * 31 + item2?.GetHashCode() ?? 0;
                hash = hash * 31 + item3?.GetHashCode() ?? 0;
                hash = hash * 31 + item4?.GetHashCode() ?? 0;
                hash = hash * 31 + item5?.GetHashCode() ?? 0;
                hash = hash * 31 + item6?.GetHashCode() ?? 0;
                hash = hash * 31 + item7?.GetHashCode() ?? 0;
                return hash;
            }
        }

        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + item1?.GetHashCode() ?? 0;
                hash = hash * 31 + item2?.GetHashCode() ?? 0;
                hash = hash * 31 + item3?.GetHashCode() ?? 0;
                hash = hash * 31 + item4?.GetHashCode() ?? 0;
                hash = hash * 31 + item5?.GetHashCode() ?? 0;
                hash = hash * 31 + item6?.GetHashCode() ?? 0;
                hash = hash * 31 + item7?.GetHashCode() ?? 0;
                hash = hash * 31 + item8?.GetHashCode() ?? 0;
                return hash;
            }
        }
    }
}
