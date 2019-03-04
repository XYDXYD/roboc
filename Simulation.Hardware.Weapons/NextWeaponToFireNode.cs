using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class NextWeaponToFireNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public ICubePositionComponent cubePositionComponent;

		public IShootingComponent shootingComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public IMisfireComponent misfireComponent;

		public IFireOrderComponent fireOrderComponent;

		public NextWeaponToFireNode()
			: this()
		{
		}
	}
}
