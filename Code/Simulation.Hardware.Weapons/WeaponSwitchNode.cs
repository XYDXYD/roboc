using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal class WeaponSwitchNode : EntityView
	{
		public IItemDescriptorComponent itemDescriptorComponent;

		public IHardwareOwnerComponent weaponOwnerComponent;

		public IWeaponActiveComponent weaponActiveComponent;

		public IWeaponCrosshairTypeComponent weaponCrosshairTypeComponent;

		public WeaponSwitchNode()
			: this()
		{
		}
	}
}
