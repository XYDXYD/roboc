using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Chaingun
{
	internal sealed class ChaingunSpinEffectNode : EntityView
	{
		public IWeaponSpinVortexEffectComponent vortexEffectComponent;

		public IWeaponHeatEffectComponent heatEffectComponent;

		public IWeaponSpinTransformComponent transformComponent;

		public IWeaponCooldownComponent cooldownComponent;

		public ChaingunSpinEffectNode()
			: this()
		{
		}
	}
}
