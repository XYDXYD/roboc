using Svelto.ECS.Legacy;

namespace Simulation
{
	internal interface IModuleGuiCooldownComponent
	{
		Dispatcher<IModuleGuiCooldownComponent, int> startCooldown
		{
			get;
		}

		Dispatcher<IModuleGuiCooldownComponent, ItemCategory> resetCooldown
		{
			get;
		}

		Dispatcher<IModuleGuiCooldownComponent, ItemCategory> notEnoughPower
		{
			get;
		}

		Dispatcher<IModuleGuiCooldownComponent, ItemCategory> cooldownStillActive
		{
			get;
		}
	}
}
