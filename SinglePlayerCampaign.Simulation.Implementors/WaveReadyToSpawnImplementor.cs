using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.Implementors
{
	internal class WaveReadyToSpawnImplementor : IReadyToSpawnWaveComponent
	{
		public DispatchOnSet<bool> ReadyToSpawn
		{
			get;
			private set;
		}

		public WaveReadyToSpawnImplementor(int entityID)
		{
			ReadyToSpawn = new DispatchOnSet<bool>(entityID);
		}
	}
}
