using System.Collections.Generic;

namespace Simulation
{
	internal class BattleStatsData
	{
		public int playerId = -1;

		public string playerName;

		public string displayName;

		public int teamId;

		public bool isFriend;

		public bool requestSent;

		public Dictionary<InGameStatId, uint> stats = new Dictionary<InGameStatId, uint>();

		public Dictionary<InGameStatId, bool> isFinal = new Dictionary<InGameStatId, bool>();
	}
}
