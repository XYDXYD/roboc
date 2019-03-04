using System;

namespace Simulation
{
	[Serializable]
	internal sealed class ScriptsOnVisibilitySettings
	{
		public float findViewportTolerance = 0.2f;

		public float loseViewportTolerance = 0.1f;

		public float cameraLowerInactivityDistance = 95f;

		public float cameraUpperInactivityDistance = 100f;

		public float maxCameraActivityDistance = 100f;
	}
}
