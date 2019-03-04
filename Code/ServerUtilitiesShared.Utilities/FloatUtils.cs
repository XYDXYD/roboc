using System;

namespace ServerUtilitiesShared.Utilities
{
	public static class FloatUtils
	{
		private const int RANGE_MIN_LOWER_BOUND = 0;

		private const int RANGE_MAX_UPPER_BOUND = 1073741824;

		private const float MAXIMUM_FLOAT_PRECISION = 1.192093E-07f;

		public static float FindMaximumPrecisionInRange(int lowerBound, int upperBound)
		{
			if (lowerBound >= upperBound)
			{
				throw new Exception("Cannot find maximum precision in the specified range [" + lowerBound + " > " + upperBound + "], lowerbound must be lower than the upperbound");
			}
			if (lowerBound < 0 || upperBound < 0 || lowerBound > 1073741823 || upperBound > 1073741824)
			{
				throw new Exception("Cannot find maximum precision in the specified range [" + lowerBound + " > " + upperBound + "], lowerbound minimum value is " + 0 + ", upperbound maximum value is " + 1073741824);
			}
			int num = FindEncompassingPow2(lowerBound);
			int num2 = FindEncompassingPow2(upperBound);
			if (upperBound - lowerBound <= 1)
			{
				return 1.192093E-07f;
			}
			return (float)((double)(num2 - num) / Math.Pow(2.0, 23.0));
		}

		private static int FindEncompassingPow2(int value)
		{
			int num = Math.Abs(value);
			int num2 = 1;
			while (num > num2)
			{
				num2 *= 2;
				if (num2 >= int.MaxValue)
				{
					throw new Exception("Cannot find encompassing POW2 for the value " + value + ". The value is too big. Max computable value is: " + 1073741824);
				}
			}
			return num2;
		}
	}
}
