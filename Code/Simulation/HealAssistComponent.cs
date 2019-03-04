using System.Collections.Generic;

namespace Simulation
{
	internal class HealAssistComponent : IHealAssistComponent
	{
		public HashSet<int> rewardedPlayers
		{
			get;
			private set;
		}

		public HealAssistComponent()
		{
			rewardedPlayers = new HashSet<int>();
		}
	}
}
