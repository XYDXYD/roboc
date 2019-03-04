using UnityEngine;

namespace Simulation
{
	internal interface IPlaySoundWhenParticleDestroyedComponent
	{
		ParticleSystem particleSys
		{
			get;
		}

		string soundToPlay
		{
			get;
		}

		int previousParticleNumber
		{
			get;
			set;
		}
	}
}
