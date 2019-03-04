using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.Components
{
	public interface IReadyToSpawnWaveComponent
	{
		DispatchOnSet<bool> ReadyToSpawn
		{
			get;
		}
	}
}
