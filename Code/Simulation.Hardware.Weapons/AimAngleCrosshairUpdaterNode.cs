using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class AimAngleCrosshairUpdaterNode : EntityView
	{
		public IItemDescriptorComponent itemDescriptorComponent;

		public IWeaponRotationTransformsComponent weaponRotationTransformsComponent;

		public IHardwareOwnerComponent ownerComponent;

		public IWeaponActiveComponent weaponActiveComponent;

		public AimAngleCrosshairUpdaterNode()
			: this()
		{
		}
	}
}
