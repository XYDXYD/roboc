using Simulation.Hardware;
using Svelto.ECS;

namespace Simulation.BattleArena
{
	internal class FusionShieldEntityView : EntityView
	{
		public ITransformComponent transformComponent;

		public IFusionShieldActivable activableComponent;

		public IFusionShieldColliderComponent fusionShieldColliderComponent;

		public IOwnerTeamComponent ownerTeamComponent;

		public FusionShieldEntityView()
			: this()
		{
		}
	}
}
