using Simulation.Hardware;
using Svelto.ECS;

namespace Simulation
{
	internal sealed class PlaySoundWhenParticleDestroyedNode : EntityView
	{
		public IPlaySoundWhenParticleDestroyedComponent playSoundWhenParticleDestroyedComponent;

		public IHardwareDisabledComponent hardwareDisabledComponent;

		public PlaySoundWhenParticleDestroyedNode()
			: this()
		{
		}
	}
}
