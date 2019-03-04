using System;
using System.Diagnostics;
using Utility;

namespace LiteNetLibExt
{
	internal class SequenceWindow
	{
		private const int WindowSize = 64;

		private int _right;

		private bool[] _seen;

		public SequenceWindow(int initialSequence)
		{
			_right = initialSequence - 1;
			_seen = new bool[64];
			for (int i = 0; i < 64; i++)
			{
				_seen[i] = true;
			}
		}

		public bool Validate(int sequence)
		{
			int num = sequence - _right;
			if (num >= 64)
			{
				return false;
			}
			int num2 = -num;
			if (num2 >= 64)
			{
				return false;
			}
			int num3 = 63 - num2;
			if (num2 >= 0 && _seen[num3])
			{
				return false;
			}
			if (sequence > _right)
			{
				int num4 = 64 - num;
				Array.Copy(_seen, num, _seen, 0, num4);
				Array.Clear(_seen, num4, 64 - num4);
				_seen[63] = true;
				_right = sequence;
			}
			else
			{
				_seen[num3] = true;
			}
			return true;
		}

		[Conditional("SEQUENCE_WINDOW_DEBUG_LOG")]
		private void DebugLog(bool warning, string text)
		{
			if (warning)
			{
				Console.LogWarning(text);
			}
			else
			{
				Console.Log(text);
			}
		}
	}
}
