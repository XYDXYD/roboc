using Svelto.ECS;

namespace Simulation
{
	internal sealed class PowerModuleNode : EntityView
	{
		public IPowerModuleOwnerComponent ownerComponent;

		public IEnabledComponent enabledComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public PowerModuleNode()
			: this()
		{
		}
	}
}
