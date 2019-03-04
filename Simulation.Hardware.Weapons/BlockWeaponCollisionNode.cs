using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class BlockWeaponCollisionNode : EntityView
	{
		public IHardwareOwnerComponent weaponOwnerComponent;

		public IHardwareDisabledComponent weaponHealthStatusComponent;

		public IFrontPositionComponent frontPositionComponent;

		public ITransformComponent transformComponent;

		public BlockWeaponCollisionNode()
			: this()
		{
		}
	}
}
