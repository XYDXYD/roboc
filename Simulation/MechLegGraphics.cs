using UnityEngine;

namespace Simulation
{
	internal sealed class MechLegGraphics
	{
		public bool canAnimate;

		public bool isAnimating;

		public bool quietAnimation;

		public bool legRetracted;

		public bool justJumped;

		public bool justLanded;

		public bool justSkidded;

		public bool isSkidding;

		public int syncGroup;

		public Vector3 startFootPos;

		public Vector3 targetFootPos;

		public float lastStepSpeed;

		public float currentNonAnimatingTime;

		public float currentTransitionTime;

		public float totalTransitionTime = 1f;

		public Vector3 stoppedLegForwardVector;

		public bool justDescending;

		public Vector3 targetJumpOffset;

		public float animationProgress => currentTransitionTime / totalTransitionTime;
	}
}
