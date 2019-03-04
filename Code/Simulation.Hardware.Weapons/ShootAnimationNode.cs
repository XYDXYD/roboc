using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal class ShootAnimationNode : EntityView
	{
		public IProjectileCreationComponent projectileCreationComponent;

		public IWeaponAnimationComponent animationComponent;

		public ShootAnimationNode()
			: this()
		{
		}
	}
}
