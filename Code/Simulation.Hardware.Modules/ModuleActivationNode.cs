using Simulation.Hardware.Weapons;
using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class ModuleActivationNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public IModuleActivationComponent activationComponent;

		public IHardwareDisabledComponent healthStatusComponent;

		public IWeaponFireCostComponent manaCostComponent;

		public IWeaponCooldownComponent cooldownComponent;

		public IModuleGuiCooldownComponent moduleCooldownGuiComponent;

		public IReadyEffectActivationComponent readyEffectActivationComponent;

		public ModuleActivationNode()
			: this()
		{
		}
	}
}
