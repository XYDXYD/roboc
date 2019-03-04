using System;

namespace Simulation.Pit
{
	internal class BattleEventStreamManagerPit : BattleEventStreamManager, IBattleEventStreamManagerPit
	{
		public event Action<int> OnNewLeader;

		public event Action<int, uint> OnStreakUpdate;

		public event Action<int> OnStreakLost;

		public void NewLeader(int userId)
		{
			if (this.OnNewLeader != null)
			{
				this.OnNewLeader(userId);
			}
		}

		public void StreakUpdate(int userId, uint streak)
		{
			if (this.OnStreakUpdate != null)
			{
				this.OnStreakUpdate(userId, streak);
			}
		}

		public void StreakLost(int userId)
		{
			if (this.OnStreakLost != null)
			{
				this.OnStreakLost(userId);
			}
		}
	}
}
