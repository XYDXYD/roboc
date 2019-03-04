using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Chaingun
{
	internal sealed class ChaingunSpinNode : EntityView
	{
		public IWeaponSpinStatsComponent spinStatsComponent;

		public IWeaponSpinEventComponent spinEventComponent;

		public IHardwareOwnerComponent weaponOwner;

		public IShootingComponent shootingComponent;

		public IWeaponCooldownComponent cooldownComponent;

		public IHardwareDisabledComponent healthStatusComponent;

		public IWeaponFireCostComponent fireCostComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public ChaingunSpinNode()
			: this()
		{
		}
	}
}
