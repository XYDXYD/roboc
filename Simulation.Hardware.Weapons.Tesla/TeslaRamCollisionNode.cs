using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Tesla
{
	internal sealed class TeslaRamCollisionNode : EntityView
	{
		public IHardwareOwnerComponent weaponOwnerComponent;

		public ITransformComponent transformComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public IWeaponActiveComponent weaponActiveComponent;

		public ICollisionComponent collisionComponent;

		public ITeslaEffectComponent effectComponent;

		public ITeslaTargetComponent teslaTargetComponent;

		public IHitSomethingComponent hitSomethingComponent;

		public IFrontPositionComponent hitPositionComponent;

		public IEntitySourceComponent entitySourceComponent;

		public TeslaRamCollisionNode()
			: this()
		{
		}
	}
}
