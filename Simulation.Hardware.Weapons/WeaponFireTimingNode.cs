using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class WeaponFireTimingNode : EntityView
	{
		public IHardwareDisabledComponent healthStatusComponent;

		public IFireTimingComponent fireTiming;

		public IItemDescriptorComponent itemDescriptorComponent;

		public IHardwareOwnerComponent weaponOwner;

		public IWeaponFireCostComponent weaponFireCostComponent;

		public IWeaponCooldownComponent weaponCooldownComponent;

		public IWeaponActiveComponent activeComponent;

		public WeaponFireTimingNode()
			: this()
		{
		}
	}
}
