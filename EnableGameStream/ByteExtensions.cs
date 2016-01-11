using System;
using System.Collections.Generic;
using System.Linq;

namespace EnableGameStream
{
	public static class ByteExtensions
	{
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
