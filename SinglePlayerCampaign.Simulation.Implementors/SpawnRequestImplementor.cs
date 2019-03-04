using SinglePlayerCampaign.DataTypes;
using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;
using System.Collections.Generic;

namespace SinglePlayerCampaign.Simulation.Implementors
{
	internal class SpawnRequestImplementor : ISpawnRequestComponent
	{
		public DispatchOnSet<IEnumerable<SpawnRequest>> spawnRequests
		{
			get;
			private set;
		}

		public SpawnRequestImplementor(int id)
		{
			spawnRequests = new DispatchOnSet<IEnumerable<SpawnRequest>>(id);
		}
	}
}
