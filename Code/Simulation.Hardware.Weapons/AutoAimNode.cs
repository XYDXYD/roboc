using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class AutoAimNode : EntityView
	{
		public ITransformComponent transformComponent;

		public IHardwareOwnerComponent ownerComponent;

		public IWeaponMuzzleFlash muzzleComponent;

		public IProjectileRangeComponent weaponRangeComponent;

		public IAimingVariablesComponent weaponAimingComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public AutoAimNode()
			: this()
		{
		}
	}
}
