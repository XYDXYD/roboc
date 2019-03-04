using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class WeaponAimNode : EntityView
	{
		public ITransformComponent transformComponent;

		public IAimSpeedComponent aimSpeedComponent;

		public IMoveLimitsComponent moveLimitsComponent;

		public IWeaponRotationTransformsComponent weaponRotationTransformsComponent;

		public IWeaponMountingModeComponent weaponMountingModeComponent;

		public IAimingVariablesComponent weaponAimingComponent;

		public IWeaponActiveComponent weaponActiveComponent;

		public IProjectileRangeComponent weaponRangeComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public IHardwareOwnerComponent ownerComponent;

		public IVisibilityComponent visibilityComponent;

		public IHardwareDisabledComponent healthStatusComponent;

		public IRigidBodyComponent rigidBodyComponent;

		public WeaponAimNode()
			: this()
		{
		}
	}
}
