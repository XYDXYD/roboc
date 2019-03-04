using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal class LockOnTargetNode : EntityView
	{
		public ILockOnTargetingParametersComponent lockOn;

		public IHardwareOwnerComponent owner;

		public IProjectileRangeComponent range;

		public IItemDescriptorComponent itemDescriptorComponent;

		public LockOnTargetNode()
			: this()
		{
		}
	}
}
