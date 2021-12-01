namespace Sysx.Hashing
{
    /// <summary>
    /// Generates strong hashcodes from supplied values.
    /// </summary>
    public static class HashCode
    {
        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
        public static int Combine<T1>(T1 value1)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + value1?.GetHashCode() ?? 0;
                return hash;
            }
        }

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
        public static int Combine<T1, T2>(T1 value1, T2 value2)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + value1?.GetHashCode() ?? 0;
                hash = hash * 31 + value2?.GetHashCode() ?? 0;
                return hash;
            }
        }

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
        public static int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + value1?.GetHashCode() ?? 0;
                hash = hash * 31 + value2?.GetHashCode() ?? 0;
                hash = hash * 31 + value3?.GetHashCode() ?? 0;
                return hash;
            }
        }

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
        public static int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + value1?.GetHashCode() ?? 0;
                hash = hash * 31 + value2?.GetHashCode() ?? 0;
                hash = hash * 31 + value3?.GetHashCode() ?? 0;
                hash = hash * 31 + value4?.GetHashCode() ?? 0;
                return hash;
            }
        }

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
        public static int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + value1?.GetHashCode() ?? 0;
                hash = hash * 31 + value2?.GetHashCode() ?? 0;
                hash = hash * 31 + value3?.GetHashCode() ?? 0;
                hash = hash * 31 + value4?.GetHashCode() ?? 0;
                hash = hash * 31 + value5?.GetHashCode() ?? 0;
                return hash;
            }
        }

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
        public static int Combine<T1, T2, T3, T4, T5, T6>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + value1?.GetHashCode() ?? 0;
                hash = hash * 31 + value2?.GetHashCode() ?? 0;
                hash = hash * 31 + value3?.GetHashCode() ?? 0;
                hash = hash * 31 + value4?.GetHashCode() ?? 0;
                hash = hash * 31 + value5?.GetHashCode() ?? 0;
                hash = hash * 31 + value6?.GetHashCode() ?? 0;
                return hash;
            }
        }

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
        public static int Combine<T1, T2, T3, T4, T5, T6, T7>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + value1?.GetHashCode() ?? 0;
                hash = hash * 31 + value2?.GetHashCode() ?? 0;
                hash = hash * 31 + value3?.GetHashCode() ?? 0;
                hash = hash * 31 + value4?.GetHashCode() ?? 0;
                hash = hash * 31 + value5?.GetHashCode() ?? 0;
                hash = hash * 31 + value6?.GetHashCode() ?? 0;
                hash = hash * 31 + value7?.GetHashCode() ?? 0;
                return hash;
            }
        }

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + value1?.GetHashCode() ?? 0;
                hash = hash * 31 + value2?.GetHashCode() ?? 0;
                hash = hash * 31 + value3?.GetHashCode() ?? 0;
                hash = hash * 31 + value4?.GetHashCode() ?? 0;
                hash = hash * 31 + value5?.GetHashCode() ?? 0;
                hash = hash * 31 + value6?.GetHashCode() ?? 0;
                hash = hash * 31 + value7?.GetHashCode() ?? 0;
                hash = hash * 31 + value8?.GetHashCode() ?? 0;
                return hash;
            }
        }

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + value1?.GetHashCode() ?? 0;
                hash = hash * 31 + value2?.GetHashCode() ?? 0;
                hash = hash * 31 + value3?.GetHashCode() ?? 0;
                hash = hash * 31 + value4?.GetHashCode() ?? 0;
                hash = hash * 31 + value5?.GetHashCode() ?? 0;
                hash = hash * 31 + value6?.GetHashCode() ?? 0;
                hash = hash * 31 + value7?.GetHashCode() ?? 0;
                hash = hash * 31 + value8?.GetHashCode() ?? 0;
                hash = hash * 31 + value9?.GetHashCode() ?? 0;
                return hash;
            }
        }

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + value1?.GetHashCode() ?? 0;
                hash = hash * 31 + value2?.GetHashCode() ?? 0;
                hash = hash * 31 + value3?.GetHashCode() ?? 0;
                hash = hash * 31 + value4?.GetHashCode() ?? 0;
                hash = hash * 31 + value5?.GetHashCode() ?? 0;
                hash = hash * 31 + value6?.GetHashCode() ?? 0;
                hash = hash * 31 + value7?.GetHashCode() ?? 0;
                hash = hash * 31 + value8?.GetHashCode() ?? 0;
                hash = hash * 31 + value9?.GetHashCode() ?? 0;
                hash = hash * 31 + value10?.GetHashCode() ?? 0;
                return hash;
            }
        }

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10, T11 value11)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + value1?.GetHashCode() ?? 0;
                hash = hash * 31 + value2?.GetHashCode() ?? 0;
                hash = hash * 31 + value3?.GetHashCode() ?? 0;
                hash = hash * 31 + value4?.GetHashCode() ?? 0;
                hash = hash * 31 + value5?.GetHashCode() ?? 0;
                hash = hash * 31 + value6?.GetHashCode() ?? 0;
                hash = hash * 31 + value7?.GetHashCode() ?? 0;
                hash = hash * 31 + value8?.GetHashCode() ?? 0;
                hash = hash * 31 + value9?.GetHashCode() ?? 0;
                hash = hash * 31 + value10?.GetHashCode() ?? 0;
                hash = hash * 31 + value11?.GetHashCode() ?? 0;
                return hash;
            }
        }

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10, T11 value11, T12 value12)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + value1?.GetHashCode() ?? 0;
                hash = hash * 31 + value2?.GetHashCode() ?? 0;
                hash = hash * 31 + value3?.GetHashCode() ?? 0;
                hash = hash * 31 + value4?.GetHashCode() ?? 0;
                hash = hash * 31 + value5?.GetHashCode() ?? 0;
                hash = hash * 31 + value6?.GetHashCode() ?? 0;
                hash = hash * 31 + value7?.GetHashCode() ?? 0;
                hash = hash * 31 + value8?.GetHashCode() ?? 0;
                hash = hash * 31 + value9?.GetHashCode() ?? 0;
                hash = hash * 31 + value10?.GetHashCode() ?? 0;
                hash = hash * 31 + value11?.GetHashCode() ?? 0;
                hash = hash * 31 + value12?.GetHashCode() ?? 0;
                return hash;
            }
        }

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10, T11 value11, T12 value12, T13 value13)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + value1?.GetHashCode() ?? 0;
                hash = hash * 31 + value2?.GetHashCode() ?? 0;
                hash = hash * 31 + value3?.GetHashCode() ?? 0;
                hash = hash * 31 + value4?.GetHashCode() ?? 0;
                hash = hash * 31 + value5?.GetHashCode() ?? 0;
                hash = hash * 31 + value6?.GetHashCode() ?? 0;
                hash = hash * 31 + value7?.GetHashCode() ?? 0;
                hash = hash * 31 + value8?.GetHashCode() ?? 0;
                hash = hash * 31 + value9?.GetHashCode() ?? 0;
                hash = hash * 31 + value10?.GetHashCode() ?? 0;
                hash = hash * 31 + value11?.GetHashCode() ?? 0;
                hash = hash * 31 + value12?.GetHashCode() ?? 0;
                hash = hash * 31 + value13?.GetHashCode() ?? 0;
                return hash;
            }
        }

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10, T11 value11, T12 value12, T13 value13, T14 value14)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + value1?.GetHashCode() ?? 0;
                hash = hash * 31 + value2?.GetHashCode() ?? 0;
                hash = hash * 31 + value3?.GetHashCode() ?? 0;
                hash = hash * 31 + value4?.GetHashCode() ?? 0;
                hash = hash * 31 + value5?.GetHashCode() ?? 0;
                hash = hash * 31 + value6?.GetHashCode() ?? 0;
                hash = hash * 31 + value7?.GetHashCode() ?? 0;
                hash = hash * 31 + value8?.GetHashCode() ?? 0;
                hash = hash * 31 + value9?.GetHashCode() ?? 0;
                hash = hash * 31 + value10?.GetHashCode() ?? 0;
                hash = hash * 31 + value11?.GetHashCode() ?? 0;
                hash = hash * 31 + value12?.GetHashCode() ?? 0;
                hash = hash * 31 + value13?.GetHashCode() ?? 0;
                hash = hash * 31 + value14?.GetHashCode() ?? 0;
                return hash;
            }
        }

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10, T11 value11, T12 value12, T13 value13, T14 value14, T15 value15)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + value1?.GetHashCode() ?? 0;
                hash = hash * 31 + value2?.GetHashCode() ?? 0;
                hash = hash * 31 + value3?.GetHashCode() ?? 0;
                hash = hash * 31 + value4?.GetHashCode() ?? 0;
                hash = hash * 31 + value5?.GetHashCode() ?? 0;
                hash = hash * 31 + value6?.GetHashCode() ?? 0;
                hash = hash * 31 + value7?.GetHashCode() ?? 0;
                hash = hash * 31 + value8?.GetHashCode() ?? 0;
                hash = hash * 31 + value9?.GetHashCode() ?? 0;
                hash = hash * 31 + value10?.GetHashCode() ?? 0;
                hash = hash * 31 + value11?.GetHashCode() ?? 0;
                hash = hash * 31 + value12?.GetHashCode() ?? 0;
                hash = hash * 31 + value13?.GetHashCode() ?? 0;
                hash = hash * 31 + value14?.GetHashCode() ?? 0;
                hash = hash * 31 + value15?.GetHashCode() ?? 0;
                return hash;
            }
        }

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10, T11 value11, T12 value12, T13 value13, T14 value14, T15 value15, T16 value16)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + value1?.GetHashCode() ?? 0;
                hash = hash * 31 + value2?.GetHashCode() ?? 0;
                hash = hash * 31 + value3?.GetHashCode() ?? 0;
                hash = hash * 31 + value4?.GetHashCode() ?? 0;
                hash = hash * 31 + value5?.GetHashCode() ?? 0;
                hash = hash * 31 + value6?.GetHashCode() ?? 0;
                hash = hash * 31 + value7?.GetHashCode() ?? 0;
                hash = hash * 31 + value8?.GetHashCode() ?? 0;
                hash = hash * 31 + value9?.GetHashCode() ?? 0;
                hash = hash * 31 + value10?.GetHashCode() ?? 0;
                hash = hash * 31 + value11?.GetHashCode() ?? 0;
                hash = hash * 31 + value12?.GetHashCode() ?? 0;
                hash = hash * 31 + value13?.GetHashCode() ?? 0;
                hash = hash * 31 + value14?.GetHashCode() ?? 0;
                hash = hash * 31 + value15?.GetHashCode() ?? 0;
                hash = hash * 31 + value16?.GetHashCode() ?? 0;
                return hash;
            }
        }

        /// <summary>
        /// Creates a hashcode from the supplied values.
        /// </summary>
        public static int Combine(params object[] values)
        {
            unchecked
            {
                int hash = 17;
                for(var i = 0; i < values.Length; i++)
                    hash = hash * 31 + values[i]?.GetHashCode() ?? 0;
                return hash;
            }
        }
    }
}
