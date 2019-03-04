using System;
using UnityEngine;

[Serializable]
public sealed class SupernovaCameraLocations
{
	public string levelName;

	public Vector3[] camPos = (Vector3[])new Vector3[2];

	public void NewCam(string _levelName, int _baseNumber, Vector3 _camPos)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (camPos.Length < 2)
		{
			camPos = (Vector3[])new Vector3[2];
		}
		levelName = _levelName;
		camPos[_baseNumber] = _camPos;
	}
}
