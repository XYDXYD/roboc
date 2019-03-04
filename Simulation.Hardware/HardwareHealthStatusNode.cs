using Svelto.ECS;

namespace Simulation.Hardware
{
	internal sealed class HardwareHealthStatusNode : EntityView
	{
		public IHardwareDisabledComponent healthStatusComponent;

		public IHardwareOwnerComponent ownerComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public HardwareHealthStatusNode()
			: this()
		{
		}
	}
}
