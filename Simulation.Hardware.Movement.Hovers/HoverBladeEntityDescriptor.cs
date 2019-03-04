using Simulation.BattleTracker;
using Svelto.ECS;

namespace Simulation.Hardware.Movement.Hovers
{
	internal class HoverBladeEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static HoverBladeEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[7]
			{
				new EntityViewBuilder<MovementTrackerNode>(),
				new EntityViewBuilder<CameraRelativeTurnDampingNode>(),
				new EntityViewBuilder<HoverAFXNode>(),
				new EntityViewBuilder<HoverBladeNode>(),
				new EntityViewBuilder<HoverGFXNode>(),
				new EntityViewBuilder<MovementStatsNode>(),
				new EntityViewBuilder<TopSpeedNode>()
			};
		}

		public HoverBladeEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
