using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class RecycleProjectileOnResetNode : EntityView
	{
		public IProjectileAliveComponent projectileAliveComponent;

		public ITransformComponent transformComponent;

		public RecycleProjectileOnResetNode()
			: this()
		{
		}
	}
}
