using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Laser
{
	internal sealed class LaserWeaponEffectsNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IHitSomethingComponent hitSomethingComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public IProjectileEffectImpactSuccessfulComponent impactSuccessful;

		public IProjectileEffectImpactSelfComponent impactSelf;

		public IProjectileEffectImpactEnvironmentComponent impactEnvironment;

		public IProjectileEffectImpactProtoniumComponent impactProtonium;

		public IProjectileEffectImpactFusionShieldComponent impactFusionShield;

		public IProjectileEffectImpactEqualizerComponent impactEqualizer;

		public LaserWeaponEffectsNode()
			: this()
		{
		}
	}
}
