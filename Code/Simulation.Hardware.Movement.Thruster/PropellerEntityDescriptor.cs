using Svelto.ECS;

namespace Simulation.Hardware.Movement.Thruster
{
	internal class PropellerEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static PropellerEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[8]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<ThrusterNode>(),
				new EntityViewBuilder<ThrusterManagerNode>(),
				new EntityViewBuilder<PropellerAudioNode>(),
				new EntityViewBuilder<PropellerGraphicEffectsNode>(),
				new EntityViewBuilder<CameraRelativeTurnDampingNode>(),
				new EntityViewBuilder<TopSpeedNode>(),
				new EntityViewBuilder<MovementStatsNode>()
			};
		}

		public PropellerEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
