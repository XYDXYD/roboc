using UnityEngine;

namespace Simulation.BattleArena.CapturePoint
{
	internal interface ICaptureRingsComponent
	{
		ParticleSystem segmentParticleSystem
		{
			get;
		}

		ParticleSystem[] captureParticleSystem
		{
			get;
		}
	}
}
