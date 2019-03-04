using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class ZoomNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IWeaponCrosshairTypeComponent crosshairComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public IZoomComponent zoomComponent;

		public ZoomNode()
			: this()
		{
		}
	}
}
