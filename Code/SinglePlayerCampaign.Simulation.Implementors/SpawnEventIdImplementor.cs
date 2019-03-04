using SinglePlayerCampaign.Simulation.Components;

namespace SinglePlayerCampaign.Simulation.Implementors
{
	internal class SpawnEventIdImplementor : ISpawnEventIdComponent
	{
		public int spawnEventId
		{
			get;
			private set;
		}

		public SpawnEventIdImplementor(int spawnEventId_)
		{
			spawnEventId = spawnEventId_;
		}
	}
}
