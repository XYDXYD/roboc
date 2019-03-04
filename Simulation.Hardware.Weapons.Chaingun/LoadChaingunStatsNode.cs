using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Chaingun
{
	internal sealed class LoadChaingunStatsNode : EntityView
	{
		public IItemDescriptorComponent itemDescriptorComponent;

		public IWeaponSpinStatsComponent spinUpComponent;

		public LoadChaingunStatsNode()
			: this()
		{
		}
	}
}
