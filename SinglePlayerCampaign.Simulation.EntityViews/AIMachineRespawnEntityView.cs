using Simulation;
using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.EntityViews
{
	internal class AIMachineRespawnEntityView : EntityView
	{
		public ISpawnEventIdComponent spawnEventIdComponent;

		public IAliveStateComponent aliveStateComponent;

		public AIMachineRespawnEntityView()
			: this()
		{
		}
	}
}
