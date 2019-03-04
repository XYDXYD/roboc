using System.Collections.Generic;

namespace Simulation.Hardware.Weapons
{
	internal class TargetTypeComparer : IEqualityComparer<TargetType>
	{
		public bool Equals(TargetType x, TargetType y)
		{
			return x == y;
		}

		public int GetHashCode(TargetType obj)
		{
			return (int)obj;
		}
	}
}
