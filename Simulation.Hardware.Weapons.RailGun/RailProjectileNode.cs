using Svelto.ECS;

namespace Simulation.Hardware.Weapons.RailGun
{
	internal sealed class RailProjectileNode : EntityView
	{
		public ITransformComponent transformComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public IWeaponDamageComponent weaponDamageComponent;

		public IProjectileDamageStatsComponent projectileDamageStats;

		public IProjectileOwnerComponent projectileOwnerComponent;

		public IEntitySourceComponent entitySourceComponent;

		public RailProjectileNode()
			: this()
		{
		}
	}
}
