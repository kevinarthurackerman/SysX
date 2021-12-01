using System;
using System.Data.SqlTypes;
using System.Runtime.InteropServices;

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
		/// <returns></returns>
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

			// Reverse the bytes to match SQL Servers ordering 
			daysSpan.Reverse();
			msecsSpan.Reverse();

			// Copy the bytes into the guid
			guidArray[^6] = daysSpan[^2];
			guidArray[^5] = daysSpan[^1];
			guidArray[^4] = msecsSpan[^4];
			guidArray[^3] = msecsSpan[^3];
			guidArray[^2] = msecsSpan[^2];
			guidArray[^1] = msecsSpan[^1];

			var a = MemoryMarshal.Read<int>(guidArray[0..4]);
			var b = MemoryMarshal.Read<short>(guidArray[4..6]);
			var c = MemoryMarshal.Read<short>(guidArray[6..8]);
			var d = guidArray[8];
			var e = guidArray[9];
			var f = guidArray[10];
			var g = guidArray[11];
			var h = guidArray[12];
			var i = guidArray[13];
			var j = guidArray[14];
			var k = guidArray[15];

			return new SqlGuid(a, b, c, d, e, f, g, h, i, j, k);
		}
#endif

#if NET48
		/// <summary>
		/// Produces a semi-sequentially ordered SQL GUID.
		/// </summary>
		/// <returns></returns>
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
