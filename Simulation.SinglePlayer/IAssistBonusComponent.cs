using System.Collections.Generic;

namespace Simulation.SinglePlayer
{
	public interface IAssistBonusComponent
	{
		Dictionary<int, int> cachedData
		{
			get;
		}

		int myTotalHealth
		{
			get;
			set;
		}
	}
}
