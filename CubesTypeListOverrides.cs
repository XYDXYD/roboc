using UnityEngine;

[CreateAssetMenu(fileName = "CubesTypeListOverrides_Default", menuName = "CubeType/CubesTypeOverride", order = 1)]
public class CubesTypeListOverrides : ScriptableObject
{
	public string[] overridePrefabHexCodes;

	public GameObject[] overridePrefabGameObjects;

	public CubesTypeListOverrides()
		: this()
	{
	}

	public bool GetCube(string cubeID, out GameObject go)
	{
		go = null;
		for (int i = 0; i < overridePrefabGameObjects.Length; i++)
		{
			if (overridePrefabHexCodes[i].CompareTo(cubeID) == 0)
			{
				go = overridePrefabGameObjects[i];
				return true;
			}
		}
		return false;
	}
}
