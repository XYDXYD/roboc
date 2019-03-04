using Svelto.DataStructures;
using System.Collections.Generic;

namespace Simulation
{
	internal class TLOG_PlayerKillTrackerDataCacheImplementor_Tencent : TLOG_IPlayerKillTrackerDataCacheComponent_Tencent
	{
		public ReadOnlyDictionary<string, PlayerDataDependency> playerDataCache
		{
			get;
			set;
		}

		public Dictionary<string, uint> playerTierCache
		{
			get;
			set;
		}
	}
}
