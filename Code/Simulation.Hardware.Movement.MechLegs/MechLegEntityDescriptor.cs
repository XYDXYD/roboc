using Simulation.BattleTracker;
using Svelto.ECS;

namespace Simulation.Hardware.Movement.MechLegs
{
	internal class MechLegEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static MechLegEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[6]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<CameraRelativeTurnDampingNode>(),
				new EntityViewBuilder<TopSpeedNode>(),
				new EntityViewBuilder<MovementStatsNode>(),
				new EntityViewBuilder<MovementTrackerNode>(),
				new EntityViewBuilder<MechLegView>()
			};
		}

		public MechLegEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
