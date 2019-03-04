using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class SmartWeaponFireNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IAimingVariablesComponent aimingVariablesComponent;

		public IWeaponAccuracyModifierComponent weaponAccuracyComponent;

		public IWeaponRotationTransformsComponent weaponRotationTransformsComponent;

		public IWeaponMuzzleFlash muzzleFlashComponent;

		public IProjectileCreationComponent projectileCreationComponent;

		public IShootingComponent shootingComponent;

		public IMisfireComponent misfireComponent;

		public ICubePositionComponent cubePositionComponent;

		public SmartWeaponFireNode()
			: this()
		{
		}
	}
}
