using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal class WeaponShootingNode : EntityView
	{
		public ICubePositionComponent cubePositionComponent;

		public IRigidBodyComponent rigidBodyComponent;

		public ITransformComponent transformComponent;

		public IShootingComponent shootingComponent;

		public IWeaponMuzzleFlash muzzleFlashComponent;

		public IWeaponRotationTransformsComponent weaponRotationTransforms;

		public IAimingVariablesComponent aimingComponent;

		public IWeaponProjectilePrefabComponent projectilePrefabComponent;

		public IWeaponCategoryComponent itemCategory;

		public IHardwareOwnerComponent weaponOwner;

		public IProjectileDamageStatsComponent projectileDamageStats;

		public IProjectileSpeedStatsComponent projectileSpeedStats;

		public IProjectileRangeComponent projectileRangeStats;

		public IWeaponAccuracyModifierComponent accuracyModifier;

		public IHitSomethingComponent hitSomethingComponent;

		public IWeaponFireCostComponent weaponFireCostComponent;

		public IZoomComponent zoomedComponent;

		public IWeaponNoFireAudioComponent noFireAudioComponent;

		public IPlayAfterEffectsComponent afterEffectsComponent;

		public IProjectileCreationComponent projectileCreationComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public IRobotShakeComponent robotShakeComponent;

		public ICameraShakeComponent cameraShakeComponent;

		public WeaponShootingNode()
			: this()
		{
		}
	}
}
