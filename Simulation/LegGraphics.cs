using UnityEngine;

namespace Simulation
{
	internal sealed class LegGraphics
	{
		public bool canAnimate;

		public bool isAnimating;

		public bool quietAnimation;

		public bool legRetracted;

		public int syncGroup;

		public Vector3 startPos;

		public Vector3 targetPos;

		public float currentTransitionTime;

		public float totalTransitionTime = 1f;

		public float animationProgress => currentTransitionTime / totalTransitionTime;
	}
}
