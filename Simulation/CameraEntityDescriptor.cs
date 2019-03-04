using Simulation.DeathEffects;
using Simulation.Hardware.Weapons;
using Simulation.SpawnEffects;
using Svelto.ECS;

namespace Simulation
{
	internal class CameraEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static CameraEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[6]
			{
				new EntityViewBuilder<CameraControlNode>(),
				new EntityViewBuilder<EmpCameraEffectsNode>(),
				new EntityViewBuilder<BlinkCameraEffectsNode>(),
				new EntityViewBuilder<CameraShakeDamageEntityView>(),
				new EntityViewBuilder<CameraDeathAnimationEntityView>(),
				new EntityViewBuilder<CameraSpawnAnimationEntityView>()
			};
		}

		public CameraEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
