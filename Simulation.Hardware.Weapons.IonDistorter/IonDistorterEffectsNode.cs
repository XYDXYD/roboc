using Svelto.ECS;

namespace Simulation.Hardware.Weapons.IonDistorter
{
	internal sealed class IonDistorterEffectsNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IHitSomethingComponent hitSomethingComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public IProjectileEffectImpactSuccessfulComponent impactSuccessful;

		public IProjectileEffectImpactSelfComponent impactSelf;

		public IProjectileEffectImpactEnvironmentComponent impactEnvironment;

		public IProjectileEffectImpactProtoniumComponent impactProtonium;

		public IProjectileEffectImpactFusionShieldComponent impactFusionShield;

		public IProjectileDeadEffectComponent deadEffect;

		public IProjectileEffectImpactEqualizerComponent impactEqualizer;

		public IonDistorterEffectsNode()
			: this()
		{
		}
	}
}
