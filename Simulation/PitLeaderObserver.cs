using System;

namespace Simulation
{
	internal class PitLeaderObserver
	{
		public Action<int> OnBecomingPitLeader = delegate
		{
		};

		public void OnBecamePitLeader(int leaderPlayerId)
		{
			OnBecomingPitLeader(leaderPlayerId);
		}
	}
}
