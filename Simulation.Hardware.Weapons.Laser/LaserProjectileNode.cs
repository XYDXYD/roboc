using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Laser
{
	internal sealed class LaserProjectileNode : EntityView
	{
		public ITransformComponent transformComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public IWeaponDamageComponent weaponDamageComponent;

		public IProjectileDamageStatsComponent projectileDamageStats;

		public IProjectileOwnerComponent projectileOwnerComponent;

		public IEntitySourceComponent entitySourceComponent;

		public LaserProjectileNode()
			: this()
		{
		}
	}
}
