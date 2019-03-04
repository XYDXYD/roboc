using System.Collections.Generic;

namespace Simulation
{
	public interface IHealAssistComponent
	{
		HashSet<int> rewardedPlayers
		{
			get;
		}
	}
}
