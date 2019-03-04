using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class ModuleSelectionNode : EntityView
	{
		public IItemDescriptorComponent itemDescriptorComponent;

		public IHardwareOwnerComponent ownerComponent;

		public ModuleSelectionNode()
			: this()
		{
		}
	}
}
