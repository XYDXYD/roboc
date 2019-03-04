using Svelto.DataStructures;
using System.Collections.Generic;
using UnityEngine;

internal interface ICubeList
{
	List<CubeTypeID> cubeKeys
	{
		get;
	}

	FasterList<CubeTypeID> cubeKeysWithoutObsolete
	{
		get;
	}

	void InitCubeCatalog(Dictionary<uint, CubeTypeData> cubeTypes);

	CubeTypeData CubeTypeDataOf(CubeTypeID index);

	CubeTypeID GetCubeType(GameObject cubeObject);

	bool IsCubeValid(CubeTypeID type);

	ItemDescriptor GetItemDescriptorFromCube(int itemTypeHashCode);

	bool GetCubeGreyedOutInTutorial(CubeTypeID id);

	BuildVisibility GetBuildModeVisibility(CubeTypeID id);

	int GetCubeHealth(uint index);

	float GetCubeHealthBoost(uint index);

	uint GetCubeCPURating(CubeTypeID id);

	ItemDescriptor GetCubeItemDescriptor(CubeTypeID id);

	bool IsIndestructible(uint id);
}
