using System.Runtime.InteropServices;

namespace Simulation
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct DoubleBits
	{
		[FieldOffset(0)]
		public long Bits;

		[FieldOffset(0)]
		public double Value;

		public DoubleBits(double v)
		{
			Bits = 0L;
			Value = v;
		}

		public DoubleBits(long bits)
		{
			Value = 0.0;
			Bits = bits;
		}

		public void Reuse(long bits)
		{
			Value = 0.0;
			Bits = bits;
		}
	}
}
