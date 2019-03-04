using System.Collections.Generic;

namespace Simulation
{
	internal class DamagedByImplementor : IDamagedByComponent
	{
		public Dictionary<int, int> damagedBy
		{
			get;
			set;
		}

		public DamagedByImplementor()
		{
			damagedBy = new Dictionary<int, int>();
		}
	}
}
