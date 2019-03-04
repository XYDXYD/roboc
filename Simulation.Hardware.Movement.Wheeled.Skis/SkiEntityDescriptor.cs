using Svelto.ECS;

namespace Simulation.Hardware.Movement.Wheeled.Skis
{
	internal class SkiEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static SkiEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[8]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<TopSpeedNode>(),
				new EntityViewBuilder<MovementStatsNode>(),
				new EntityViewBuilder<WheelColliderNode>(),
				new EntityViewBuilder<WheelNode>(),
				new EntityViewBuilder<SkiGraphicsNode>(),
				new EntityViewBuilder<SkiAudioNode>(),
				new EntityViewBuilder<CameraRelativeTurnDampingNode>()
			};
		}

		public SkiEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
