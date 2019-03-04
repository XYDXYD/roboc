using System;

namespace Simulation
{
	[Serializable]
	internal sealed class PlayerMarkerConfig
	{
		public float maxDistance = 286f;

		public float minDistance = 20f;

		public float maxDistanceAlpha = 0.2f;

		public float minDistanceAlpha = 0.9f;

		public float maxDistanceScale = 0.5f;

		public float minDistanceScale = 1f;
	}
}
