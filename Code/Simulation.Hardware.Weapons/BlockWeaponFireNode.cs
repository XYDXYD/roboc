using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class BlockWeaponFireNode : EntityView
	{
		public IWeaponMuzzleFlash muzzleFlashComponent;

		public IHardwareOwnerComponent weaponOwnerComponent;

		public IHardwareDisabledComponent weaponHealthStatusComponent;

		public BlockWeaponFireNode()
			: this()
		{
		}
	}
}
