using UnityEngine;

namespace Simulation.SpawnEffects
{
	internal interface ISpawnAnimationComponent
	{
		float pitch
		{
			get;
		}

		float distance
		{
			get;
		}

		float positionDuration
		{
			get;
		}

		AnimationCurve positionCurve
		{
			get;
		}

		float rotationDuration
		{
			get;
		}

		AnimationCurve rotationCurve
		{
			get;
		}

		SimulationCamera controlScript
		{
			get;
		}
	}
}
