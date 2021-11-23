namespace Sysx.Hashing
{
    /// <summary>
    /// Generates strong hashcodes from supplied values on the class.
    /// </summary>
    public static class HashCode
    {
        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
        public static int Combine<T1>(T1 item1)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + item1?.GetHashCode() ?? 0;
                return hash;
            }
        }

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
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

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
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

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
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

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
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

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
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

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
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

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
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

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, T9 item9)
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
                hash = hash * 31 + item9?.GetHashCode() ?? 0;
                return hash;
            }
        }

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, T9 item9, T10 item10)
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
                hash = hash * 31 + item9?.GetHashCode() ?? 0;
                hash = hash * 31 + item10?.GetHashCode() ?? 0;
                return hash;
            }
        }

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, T9 item9, T10 item10, T11 item11)
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
                hash = hash * 31 + item9?.GetHashCode() ?? 0;
                hash = hash * 31 + item10?.GetHashCode() ?? 0;
                hash = hash * 31 + item11?.GetHashCode() ?? 0;
                return hash;
            }
        }

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, T9 item9, T10 item10, T11 item11, T12 item12)
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
                hash = hash * 31 + item9?.GetHashCode() ?? 0;
                hash = hash * 31 + item10?.GetHashCode() ?? 0;
                hash = hash * 31 + item11?.GetHashCode() ?? 0;
                hash = hash * 31 + item12?.GetHashCode() ?? 0;
                return hash;
            }
        }

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, T9 item9, T10 item10, T11 item11, T12 item12, T13 item13)
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
                hash = hash * 31 + item9?.GetHashCode() ?? 0;
                hash = hash * 31 + item10?.GetHashCode() ?? 0;
                hash = hash * 31 + item11?.GetHashCode() ?? 0;
                hash = hash * 31 + item12?.GetHashCode() ?? 0;
                hash = hash * 31 + item13?.GetHashCode() ?? 0;
                return hash;
            }
        }

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, T9 item9, T10 item10, T11 item11, T12 item12, T13 item13, T14 item14)
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
                hash = hash * 31 + item9?.GetHashCode() ?? 0;
                hash = hash * 31 + item10?.GetHashCode() ?? 0;
                hash = hash * 31 + item11?.GetHashCode() ?? 0;
                hash = hash * 31 + item12?.GetHashCode() ?? 0;
                hash = hash * 31 + item13?.GetHashCode() ?? 0;
                hash = hash * 31 + item14?.GetHashCode() ?? 0;
                return hash;
            }
        }

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, T9 item9, T10 item10, T11 item11, T12 item12, T13 item13, T14 item14, T15 item15)
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
                hash = hash * 31 + item9?.GetHashCode() ?? 0;
                hash = hash * 31 + item10?.GetHashCode() ?? 0;
                hash = hash * 31 + item11?.GetHashCode() ?? 0;
                hash = hash * 31 + item12?.GetHashCode() ?? 0;
                hash = hash * 31 + item13?.GetHashCode() ?? 0;
                hash = hash * 31 + item14?.GetHashCode() ?? 0;
                hash = hash * 31 + item15?.GetHashCode() ?? 0;
                return hash;
            }
        }

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, T9 item9, T10 item10, T11 item11, T12 item12, T13 item13, T14 item14, T15 item15, T16 item16)
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
                hash = hash * 31 + item9?.GetHashCode() ?? 0;
                hash = hash * 31 + item10?.GetHashCode() ?? 0;
                hash = hash * 31 + item11?.GetHashCode() ?? 0;
                hash = hash * 31 + item12?.GetHashCode() ?? 0;
                hash = hash * 31 + item13?.GetHashCode() ?? 0;
                hash = hash * 31 + item14?.GetHashCode() ?? 0;
                hash = hash * 31 + item15?.GetHashCode() ?? 0;
                hash = hash * 31 + item16?.GetHashCode() ?? 0;
                return hash;
            }
        }
    }
}
