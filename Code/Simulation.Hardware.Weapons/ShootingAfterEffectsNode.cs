using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class ShootingAfterEffectsNode : EntityView
	{
		public ICubePositionComponent cubePositionComponent;

		public IHardwareOwnerComponent weaponOwner;

		public IRigidBodyComponent rigidBodyComponent;

		public ITransformComponent transformComponent;

		public IWeaponRotationTransformsComponent weaponRotationTransforms;

		public IZoomComponent zoomedComponent;

		public IWeaponMuzzleFlash muzzleFlashComponent;

		public IWeaponFiringAudioComponent firingAudioComponent;

		public IRecoilForceComponent recoilForceComponent;

		public ICameraShakeComponent camShakeComponent;

		public IPlayAfterEffectsComponent afterEffectsComponent;

		public IVisibilityComponent visibilityComponent;

		public IAimingVariablesComponent weaponAimingComponent;

		public IProjectileRangeComponent weaponRangeComponent;

		public IWeaponFireCostComponent weaponFireCostComponent;

		public IWeaponCooldownComponent weaponCooldownComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public ShootingAfterEffectsNode()
			: this()
		{
		}
	}
}
