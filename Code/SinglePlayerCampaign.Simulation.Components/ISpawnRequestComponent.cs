using SinglePlayerCampaign.DataTypes;
using Svelto.ECS;
using System.Collections.Generic;

namespace SinglePlayerCampaign.Simulation.Components
{
	internal interface ISpawnRequestComponent
	{
		DispatchOnSet<IEnumerable<SpawnRequest>> spawnRequests
		{
			get;
		}
	}
}
