using Svelto.ECS;

namespace Simulation.Hardware.Movement.Thruster
{
	internal class ThrusterEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static ThrusterEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[7]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<ThrusterNode>(),
				new EntityViewBuilder<ThrusterManagerNode>(),
				new EntityViewBuilder<ThrusterAudioNode>(),
				new EntityViewBuilder<CameraRelativeTurnDampingNode>(),
				new EntityViewBuilder<TopSpeedNode>(),
				new EntityViewBuilder<MovementStatsNode>()
			};
		}

		public ThrusterEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
