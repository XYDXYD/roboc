using Simulation.BattleTracker;
using Svelto.ECS;

namespace Simulation.Hardware.Movement.InsectLegs
{
	internal class InsectLegEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static InsectLegEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[6]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<CameraRelativeTurnDampingNode>(),
				new EntityViewBuilder<TopSpeedNode>(),
				new EntityViewBuilder<MovementStatsNode>(),
				new EntityViewBuilder<MovementTrackerNode>(),
				new EntityViewBuilder<InsectLegView>()
			};
		}

		public InsectLegEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
