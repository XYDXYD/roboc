using UnityEngine;

namespace Simulation.DeathEffects
{
	internal interface IDeathAnimationComponent
	{
		float pitch
		{
			get;
		}

		float distance
		{
			get;
		}

		float ratioAboveRobot
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
