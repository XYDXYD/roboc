using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class CrosshairWeaponNoFireStateTrackerNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IWeaponFireCostComponent manaComponent;

		public IWeaponCooldownComponent cooldownComponent;

		public IWeaponActiveComponent activeComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public CrosshairWeaponNoFireStateTrackerNode()
			: this()
		{
		}
	}
}
