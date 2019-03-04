using SinglePlayerCampaign.DataTypes;
using SinglePlayerCampaign.Simulation.Components;

namespace SinglePlayerCampaign.Simulation.Implementors
{
	internal class SpawnDataImplementor : ISpawnDataComponent
	{
		public SpawnEvent[] spawnEvents
		{
			get;
			private set;
		}

		public SpawnDataImplementor(SpawnEvent[] spawnEvents_)
		{
			spawnEvents = spawnEvents_;
		}
	}
}
