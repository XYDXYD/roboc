using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Nano
{
	internal sealed class NanoCrosshairUpdaterNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IWeaponMuzzleFlash muzzleComponent;

		public IAimingVariablesComponent aimingComponent;

		public IProjectileRangeComponent rangeComponent;

		public IHardwareDisabledComponent healthStatusComponent;

		public IWeaponActiveComponent weaponActiveComponent;

		public NanoCrosshairUpdaterNode()
			: this()
		{
		}
	}
}
