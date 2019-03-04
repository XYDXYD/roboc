using System;

[Serializable]
internal sealed class OptimizationsOnVisibilitySettings
{
	public float findViewportTolerance = 0.2f;

	public float loseViewportTolerance = 0.1f;

	public float maxCameraActivityDistance = 100f;
}
