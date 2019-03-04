using Simulation.SinglePlayer;
using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.EntityViews
{
	internal class AIMachineDespawnEntityView : EntityView
	{
		public ISpawnEventIdComponent spawnEventIdComponent;

		public IAIBotIdDataComponent aiBotIdDataComponent;

		public AIMachineDespawnEntityView()
			: this()
		{
		}
	}
}
