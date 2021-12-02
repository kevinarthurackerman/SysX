using System;
using System.Data.SqlTypes;
#if NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
using System.Runtime.InteropServices;
#endif

namespace Sysx.Identity
{
	/// <summary>
	/// Generates semi-sequentially ordered GUIDs that will sort correctly in memory and SQL Server.
	/// </summary>
    public static class SequentialGuid
    {
		private static readonly long baseDateTicks = new DateTime(1900, 1, 1).Ticks;

#if NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
		/// <summary>
		/// Produces a semi-sequentially ordered SQL GUID.
		/// </summary>
		public static SqlGuid Next()
		{
			Span<byte> guidArray = stackalloc byte[16];
			Guid.NewGuid().TryWriteBytes(guidArray);

			var now = DateTime.UtcNow;

			// Get the days and milliseconds which will be used to build the byte string 
			var days = new TimeSpan(now.Ticks - baseDateTicks).Days;
			var msecs = (long)now.TimeOfDay.TotalMilliseconds;

			// Convert to byte spans
			Span<byte> daysSpan = stackalloc byte[4];
			MemoryMarshal.Write(daysSpan, ref days);

			Span<byte> msecsSpan = stackalloc byte[8];
			MemoryMarshal.Write(msecsSpan, ref msecs);

			return new SqlGuid(
				MemoryMarshal.Read<int>(guidArray[0..4]),
				MemoryMarshal.Read<short>(guidArray[4..6]),
				MemoryMarshal.Read<short>(guidArray[6..8]),
				guidArray[8],
				guidArray[9],
				daysSpan[1],
				daysSpan[0],
				msecsSpan[3],
				msecsSpan[2],
				msecsSpan[1],
				msecsSpan[0]);
		}
#endif

#if NET48
		/// <summary>
		/// Produces a semi-sequentially ordered SQL GUID.
		/// </summary>
		public static SqlGuid Next()
		{
			var guidArray = Guid.NewGuid().ToByteArray();

			var now = DateTime.UtcNow;
			
			// Get the days and milliseconds which will be used to build the byte string 
			var days = new TimeSpan(now.Ticks - baseDateTicks);
			var msecs = now.TimeOfDay;
			
			// Convert to byte arrays
			var daysArray = BitConverter.GetBytes(days.Days);
			var msecsArray = BitConverter.GetBytes((long)msecs.TotalMilliseconds);

			// Reverse the bytes to match SQL Servers ordering 
			Array.Reverse(daysArray);
			Array.Reverse(msecsArray);

			// Copy the bytes into the guid 
			Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
			Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

			return new SqlGuid(guidArray);
		}
#endif
	}
}
