using UnityEngine;

namespace Simulation.Hardware.Movement.Thruster
{
	internal interface IThrusterParticleEffectsComponent
	{
		ParticleSystem[] particleSystems
		{
			get;
		}
	}
}
