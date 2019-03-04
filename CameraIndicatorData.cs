using Simulation;
using UnityEngine;

internal class CameraIndicatorData : MonoBehaviour, ICameraIndicatorDataComponent
{
	[SerializeField]
	private float radiusMultiplier;

	public CameraIndicatorData()
		: this()
	{
	}

	public float GetRadiusMultiplier()
	{
		return radiusMultiplier;
	}
}
