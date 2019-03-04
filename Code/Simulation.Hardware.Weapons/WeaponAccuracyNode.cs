using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class WeaponAccuracyNode : EntityView
	{
		public IItemDescriptorComponent itemDescriptorComponent;

		public IWeaponCategoryComponent itemCategory;

		public IWeaponAccuracyStatsComponent weaponAccuracyStats;

		public IWeaponAccuracyModifierComponent weaponAccuracyModifier;

		public IWeaponMovementComponent weaponMovement;

		public ITransformComponent transformComponent;

		public IAimingVariablesComponent aimingComponent;

		public IShootingComponent shootingComponent;

		public IHardwareOwnerComponent ownerComponent;

		public IWeaponActiveComponent weaponActiveComponent;

		public IHardwareDisabledComponent disabledComponent;

		public WeaponAccuracyNode()
			: this()
		{
		}
	}
}
