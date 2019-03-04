using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class WeaponDestructionNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IHardwareDisabledComponent healthStatusComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public WeaponDestructionNode()
			: this()
		{
		}
	}
}
