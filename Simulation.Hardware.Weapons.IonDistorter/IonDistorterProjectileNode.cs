using Svelto.ECS;

namespace Simulation.Hardware.Weapons.IonDistorter
{
	internal sealed class IonDistorterProjectileNode : EntityView
	{
		public ITransformComponent transformComponent;

		public IIonDistorterProjectileConeComponent coneComponent;

		public IIonDistorterCollisonComponent collisionComponent;

		public IEntitySourceComponent entitySourceComponent;

		public IProjectileOwnerComponent projectileOwnerComponent;

		public IWeaponDamageComponent weaponDamageComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public IHitSomethingComponent hitSomethingComponent;

		public IProjectileDamageStatsComponent projectileDamageStatsComponent;

		public IProjectileTimeComponent projectileTimeComponent;

		public IProjectileRangeComponent projectileRangeComponent;

		public IProjectileAliveComponent projectileAliveComponent;

		public IProjectileMovementStatsComponent projectileMovementStatsComponent;

		public IProjectileStateComponent projectileStateComponent;

		public IonDistorterProjectileNode()
			: this()
		{
		}
	}
}
