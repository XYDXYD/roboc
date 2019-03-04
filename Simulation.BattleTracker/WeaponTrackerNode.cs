using Simulation.Hardware;
using Svelto.ECS;

namespace Simulation.BattleTracker
{
	internal sealed class WeaponTrackerNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IWeaponActiveComponent weaponActiveComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public WeaponTrackerNode()
			: this()
		{
		}
	}
}
