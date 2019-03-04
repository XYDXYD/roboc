using SinglePlayerCampaign.DataTypes;

namespace SinglePlayerCampaign.Simulation.Components
{
	internal interface ISpawnDataComponent
	{
		SpawnEvent[] spawnEvents
		{
			get;
		}
	}
}
