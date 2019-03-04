using Mothership;
using System.Collections.Generic;

namespace Simulation.Analytics
{
	internal class WorldSwitchingSimulationAnalytics : WorldSwitchingAnalytics
	{
		public IDictionary<int, int> tdmTeamKills
		{
			get;
			private set;
		}

		public void UpdateTDMTeamKills(IDictionary<int, int> tdmTeamKills)
		{
			this.tdmTeamKills = tdmTeamKills;
		}
	}
}
