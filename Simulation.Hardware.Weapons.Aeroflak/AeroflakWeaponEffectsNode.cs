using Svelto.ECS;

namespace Simulation.Hardware.Weapons.AeroFlak
{
	internal sealed class AeroflakWeaponEffectsNode : EntityView
	{
		public IHitSomethingComponent hitSomethingComponent;

		public IProjectileEffectImpactSuccessfulComponent impactSuccessful;

		public IProjectileEffectExplosionComponent impactExplosion;

		public IProjectileEffectImpactSelfComponent impactSelf;

		public IProjectileEffectImpactEnvironmentComponent impactEnvironment;

		public IProjectileEffectImpactProtoniumComponent impactProtonium;

		public IProjectileEffectImpactFusionShieldComponent impactFusionShield;

		public IAeroflakProjectileStatsComponent aeroflakStats;

		public IHardwareOwnerComponent ownerComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public IProjectileEffectImpactEqualizerComponent impactEqualizer;

		public AeroflakWeaponEffectsNode()
			: this()
		{
		}
	}
}
