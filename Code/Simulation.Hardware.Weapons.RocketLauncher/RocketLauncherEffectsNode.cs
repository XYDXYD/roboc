using Svelto.ECS;

namespace Simulation.Hardware.Weapons.RocketLauncher
{
	internal sealed class RocketLauncherEffectsNode : EntityView
	{
		public IHitSomethingComponent hitSomethingComponent;

		public IHardwareOwnerComponent ownerComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public IProjectileEffectImpactSuccessfulComponent impactSuccessful;

		public IProjectileEffectImpactSelfComponent impactSelf;

		public IProjectileEffectImpactEnvironmentComponent impactEnvironment;

		public IProjectileEffectImpactProtoniumComponent impactProtonium;

		public IProjectileEffectImpactFusionShieldComponent impactFusionShield;

		public IProjectileEffectImpactEqualizerComponent impactEqualizer;

		public RocketLauncherEffectsNode()
			: this()
		{
		}
	}
}
