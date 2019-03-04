using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.EntityViews
{
	internal class AIMachineKillRequirementEntityView : EntityView
	{
		public IIsKillRequirementComponent isKillRequirementComponent;

		public ISpawnEventIdComponent spawnEventIdComponent;

		public AIMachineKillRequirementEntityView()
			: this()
		{
		}
	}
}
