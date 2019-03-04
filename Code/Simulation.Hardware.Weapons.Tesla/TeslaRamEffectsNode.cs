using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Tesla
{
	internal sealed class TeslaRamEffectsNode : EntityView
	{
		public IGameObjectComponent gameObjectComponent;

		public IWeaponActiveComponent weaponActiveComponent;

		public IHitSomethingComponent hitComponent;

		public ITeslaEffectComponent effectComponent;

		public IHardwareOwnerComponent weaponOwnerComponent;

		public ITransformComponent transformComponent;

		public IProjectileEffectImpactEnvironmentComponent impactEnvironment;

		public IProjectileEffectImpactSuccessfulComponent impactSuccessful;

		public IProjectileEffectImpactProtoniumComponent impactProtonium;

		public IProjectileEffectImpactEqualizerComponent impactEqualizer;

		public IAudioOnEnabledComponent enabledAudio;

		public IAudioOnDisabledComponent disabledAudio;

		public IItemDescriptorComponent itemDescriptorComponent;

		public ITeslaBladesComponent teslaBladesComponent;

		public IHardwareDisabledComponent hardwareDisabledComponent;

		public TeslaRamEffectsNode()
			: this()
		{
		}
	}
}
