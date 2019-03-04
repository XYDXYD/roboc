using System;

namespace Simulation.Pit
{
	internal interface IBattleEventStreamManagerPit
	{
		event Action<int> OnNewLeader;

		event Action<int, uint> OnStreakUpdate;

		event Action<int> OnStreakLost;

		void NewLeader(int userId);

		void StreakUpdate(int userId, uint streak);

		void StreakLost(int userId);
	}
}
