using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Chaingun
{
	internal sealed class ChaingunSpinAudioNode : EntityView
	{
		public IWeaponSpinEventComponent spinEventComponent;

		public IWeaponSpinAudioComponent spinAudioComponent;

		public ITransformComponent transformComponent;

		public IWeaponFiringAudioComponent firingAudioComponent;

		public ChaingunSpinAudioNode()
			: this()
		{
		}
	}
}
