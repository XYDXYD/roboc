using Simulation.BattleArena.Equalizer;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.EntityViews
{
	internal class AIMachineEntityView : EntityView
	{
		public IMachineMapComponent machineMapComponent;

		public AIMachineEntityView()
			: this()
		{
		}
	}
}
