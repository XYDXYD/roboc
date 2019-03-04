using Simulation.Hardware.Weapons;
using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class ModuleCooldownNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public IWeaponFireCostComponent manaCostComponent;

		public IWeaponCooldownComponent cooldownComponent;

		public IModuleGuiCooldownComponent moduleCooldownGuiComponent;

		public IModuleConfirmActivationComponent confirmActivationComponent;

		public ModuleCooldownNode()
			: this()
		{
		}
	}
}
