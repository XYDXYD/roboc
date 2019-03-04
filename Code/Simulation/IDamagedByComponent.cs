using System.Collections.Generic;

namespace Simulation
{
	internal interface IDamagedByComponent
	{
		Dictionary<int, int> damagedBy
		{
			get;
			set;
		}
	}
}
