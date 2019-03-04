using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class HealingProjectileEffectNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public IHitSomethingComponent hitSomethingComponent;

		public IProjectileEffectImpactSuccessfulComponent impactSuccessfulComponent;

		public IProjectileEffectImpactEnvironmentComponent hitEnvironmentComponent;

		public IProjectileEffectImpactMissComponent hitMissComponent;

		public HealingProjectileEffectNode()
			: this()
		{
		}
	}
}
