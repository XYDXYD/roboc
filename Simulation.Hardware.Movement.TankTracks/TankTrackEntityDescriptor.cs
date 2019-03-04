using Simulation.BattleTracker;
using Svelto.ECS;

namespace Simulation.Hardware.Movement.TankTracks
{
	internal class TankTrackEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static TankTrackEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[10]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<TankTrackNode>(),
				new EntityViewBuilder<SingleTrackActivatorNode>(),
				new EntityViewBuilder<CameraRelativeTurnDampingNode>(),
				new EntityViewBuilder<TankTrackAudioNode>(),
				new EntityViewBuilder<TankTrackGraphicsNode>(),
				new EntityViewBuilder<WheelColliderNode>(),
				new EntityViewBuilder<TopSpeedNode>(),
				new EntityViewBuilder<MovementStatsNode>(),
				new EntityViewBuilder<MovementTrackerNode>()
			};
		}

		public TankTrackEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
