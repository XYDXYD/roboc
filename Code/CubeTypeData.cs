using System;
using UnityEngine;

[Serializable]
public sealed class CubeTypeData
{
	public string nameStrKey;

	public bool active;

	public string spriteName;

	public GameObject prefab;

	[HideInInspector]
	[Obsolete]
	public string description;

	[HideInInspector]
	[Obsolete]
	public string stat1;

	[HideInInspector]
	[Obsolete]
	public string stat2;

	[HideInInspector]
	[Obsolete]
	public string stat3;

	[HideInInspector]
	[Obsolete]
	public string stat4;

	[HideInInspector]
	[Obsolete]
	public string stat5;

	[HideInInspector]
	[Obsolete]
	public string stat6;

	public PersistentCubeData cubeData;

	[HideInInspector]
	public string itemCode;

	[HideInInspector]
	public uint itemCodeValue;

	internal CubeTypeID requiredCubeId => cubeData.variantOf;

	public CubeTypeData()
	{
	}

	public CubeTypeData(CubeTypeData copy)
	{
		nameStrKey = copy.nameStrKey;
		active = copy.active;
		spriteName = copy.spriteName;
		prefab = copy.prefab;
		itemCode = copy.itemCode;
		itemCodeValue = copy.itemCodeValue;
		cubeData = new PersistentCubeData();
	}
}
