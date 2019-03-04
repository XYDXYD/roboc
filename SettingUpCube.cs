using UnityEngine;

internal sealed class SettingUpCube
{
	public Transform transform;

	public InstantiatedCube instance;

	internal SettingUpCube(Transform t, InstantiatedCube instantiatedCube)
	{
		transform = t;
		instance = instantiatedCube;
	}
}
