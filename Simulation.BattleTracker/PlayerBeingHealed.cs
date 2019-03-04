using System;

namespace Simulation.BattleTracker
{
	public class PlayerBeingHealed
	{
		public DateTime LastHealTime;

		public int StartHealthPercent;

		public PlayerBeingHealed(DateTime lastHealTime, int startHealthPercent)
		{
			LastHealTime = lastHealTime;
			StartHealthPercent = startHealthPercent;
		}
	}
}
