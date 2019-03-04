using Svelto.DataStructures;
using System.Collections.Generic;

namespace Simulation
{
	internal interface TLOG_IPlayerKillTrackerDataCacheComponent_Tencent
	{
		ReadOnlyDictionary<string, PlayerDataDependency> playerDataCache
		{
			get;
			set;
		}

		Dictionary<string, uint> playerTierCache
		{
			get;
			set;
		}
	}
}
