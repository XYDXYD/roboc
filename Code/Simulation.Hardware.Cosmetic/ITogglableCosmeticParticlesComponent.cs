using UnityEngine;

namespace Simulation.Hardware.Cosmetic
{
	internal interface ITogglableCosmeticParticlesComponent
	{
		ParticleSystem idleLoopParticles
		{
			get;
		}

		ParticleSystem stopParticles
		{
			get;
		}
	}
}
