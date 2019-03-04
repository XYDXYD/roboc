using Simulation.Hardware.Weapons;
using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class ModuleGUIEntityView : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public IModuleGuiCooldownComponent moduleCooldownGuiComponent;

		public IReadyEffectActivationComponent readyEffectActivationComponent;

		public IWeaponFireCostComponent manaCostComponent;

		public IWeaponCooldownComponent cooldownComponent;

		public ModuleGUIEntityView()
			: this()
		{
		}
	}
}
