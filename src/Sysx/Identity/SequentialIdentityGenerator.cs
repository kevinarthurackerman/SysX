using System;
using System.Data.SqlTypes;

namespace Sysx.Identity
{
	/// <summary>
	/// Generates semi-sequentially ordered GUIDs that will sort correctly in memory and SQL Server.
	/// </summary>
    public static class SequentialIdentityGenerator
    {
		private static readonly long baseDateTicks = new DateTime(1900, 1, 1).Ticks;

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
			
			// Convert to a byte array
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
	}
}
