using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal class SharedSpinDataNode : EntityView
	{
		public ISharedChaingunSpinComponent sharedChaingunSpinComponent;

		public IWeaponSpinStatsComponent weaponSpinComponent;

		public IHardwareOwnerComponent hardwareOwner;

		public SharedSpinDataNode()
			: this()
		{
		}
	}
}
