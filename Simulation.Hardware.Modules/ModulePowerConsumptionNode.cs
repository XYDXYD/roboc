using Simulation.Hardware.Weapons;
using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class ModulePowerConsumptionNode : EntityView
	{
		public IModuleConfirmActivationComponent confirmActivationComponent;

		public IWeaponFireCostComponent manaCostComponent;

		public ModulePowerConsumptionNode()
			: this()
		{
		}
	}
}
