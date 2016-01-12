using System;
using System.Collections.Generic;
using System.Linq;

namespace EnableGameStream
{
	/// <summary>
	/// Extension for byte[]
	/// </summary>
	public static class ByteExtensions
	{
		/// <summary>
		/// Scan the byte arrray for the pattern
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="pattern"></param>
		/// <param name="startIndex"></param>
		/// <returns></returns>
		public static List<int> IndexOfSequence(this byte[] buffer, byte[] pattern, int startIndex = 0)
		{
			var positions = new List<int>();
			int i = Array.IndexOf(buffer, pattern[0], startIndex);
			var segment = new byte[pattern.Length];
			while (i >= 0 && i <= buffer.Length - pattern.Length)
			{
				Buffer.BlockCopy(buffer, i, segment, 0, pattern.Length);
				if (segment.SequenceEqual(pattern))
				{
					positions.Add(i);
				}
				i = Array.IndexOf(buffer, pattern[0], i + pattern.Length);
			}
			return positions;
		}
	}
}
