using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Tesla
{
	internal sealed class TeslaRamNode : EntityView
	{
		public ITransformComponent transformComponent;

		public ICubePositionComponent cubePositionComponent;

		public IHardwareOwnerComponent weaponOwnerComponent;

		public ICameraShakeComponent cameraShakeComponent;

		public IRobotShakeComponent robotShakeComponent;

		public IProjectileDamageStatsComponent damageStatsComponent;

		public IWeaponActiveComponent weaponActiveComponent;

		public ITeslaEffectComponent effectComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public IShootingComponent shootingComponent;

		public IHitSomethingComponent hitSomethingComponent;

		public ITeslaTargetComponent teslaTargetComponent;

		public IWeaponFireCostComponent weaponFireCostComponent;

		public IHardwareDisabledComponent healthStatusComponent;

		public TeslaRamNode()
			: this()
		{
		}
	}
}
