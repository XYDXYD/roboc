using Simulation.BattleTracker;
using Svelto.ECS;

namespace Simulation.Hardware.Movement.Wheeled.Wheels
{
	internal class WheelEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static WheelEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[9]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<WheelNode>(),
				new EntityViewBuilder<CameraRelativeTurnDampingNode>(),
				new EntityViewBuilder<WheelAudioNode>(),
				new EntityViewBuilder<WheelGraphicsNode>(),
				new EntityViewBuilder<WheelColliderNode>(),
				new EntityViewBuilder<TopSpeedNode>(),
				new EntityViewBuilder<MovementStatsNode>(),
				new EntityViewBuilder<MovementTrackerNode>()
			};
		}

		public WheelEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
