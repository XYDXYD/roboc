using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class CrosshairWeaponTrackerNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IWeaponAccuracyModifierComponent accuracyModifierComponent;

		public IWeaponActiveComponent activeComponent;

		public IHardwareDisabledComponent EnableComponent;

		public CrosshairWeaponTrackerNode()
			: this()
		{
		}
	}
}
