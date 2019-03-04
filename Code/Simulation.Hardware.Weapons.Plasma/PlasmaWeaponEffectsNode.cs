using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Plasma
{
	internal sealed class PlasmaWeaponEffectsNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IHitSomethingComponent hitSomethingComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public IProjectileEffectImpactSuccessfulComponent impactSuccessful;

		public IProjectileEffectImpactSelfComponent impactSelf;

		public IProjectileEffectImpactProtoniumComponent impactProtonium;

		public IProjectileEffectImpactFusionShieldComponent impactFusionShield;

		public IProjectileEffectImpactSecondaryComponent impactSecondary;

		public IProjectileEffectImpactEqualizerComponent impactEqualizer;

		public PlasmaWeaponEffectsNode()
			: this()
		{
		}
	}
}
