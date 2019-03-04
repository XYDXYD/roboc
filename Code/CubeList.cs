using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Utility;

internal sealed class CubeList : ICubeList
{
	private Dictionary<CubeTypeID, CubeTypeData> _cubeTypes;

	private Dictionary<string, CubeTypeID> _cubeName;

	private Dictionary<int, CubeTypeID> _itemDescriptorHashCodeToCubeTypeID = new Dictionary<int, CubeTypeID>();

	private bool _initialized;

	[Inject]
	public IServiceRequestFactory serviceFactory
	{
		private get;
		set;
	}

	public List<CubeTypeID> cubeKeys
	{
		get;
		private set;
	}

	public FasterList<CubeTypeID> cubeKeysWithoutObsolete
	{
		get;
		private set;
	}

	public CubeList()
	{
		_cubeTypes = null;
		_initialized = false;
	}

	public void InitCubeCatalog(Dictionary<uint, CubeTypeData> cubeTypes)
	{
		cubeKeysWithoutObsolete = new FasterList<CubeTypeID>();
		_cubeTypes = new Dictionary<CubeTypeID, CubeTypeData>();
		_cubeName = new Dictionary<string, CubeTypeID>();
		foreach (KeyValuePair<uint, CubeTypeData> cubeType in cubeTypes)
		{
			CubeTypeID cubeTypeID = new CubeTypeID(cubeType.Key);
			if (cubeType.Value.prefab == null)
			{
				Console.LogError("Cube prefab not assigned! " + cubeType.Value.nameStrKey);
			}
			string name = cubeType.Value.prefab.get_name();
			if (_cubeName.TryGetValue(name, out CubeTypeID value))
			{
				Console.LogError("Cube prefab has same name as an existing one! Assigned same prefab twice? Existing: " + value.ToHexString() + ", adding: " + cubeTypeID.ToHexString() + " " + name);
			}
			_cubeTypes.Add(cubeTypeID, cubeType.Value);
			_cubeName.Add(name, cubeTypeID);
		}
		LoadCubes();
		cubeKeys = new List<CubeTypeID>(_cubeTypes.Keys);
	}

	private void LoadCubes()
	{
		ILoadCubeListRequest loadCubeListRequest = serviceFactory.Create<ILoadCubeListRequest>();
		loadCubeListRequest.SetAnswer(new ServiceAnswer<ReadOnlyDictionary<CubeTypeID, CubeListData>>(ProcessLoadedCubeList, OnLoadingFailed));
		loadCubeListRequest.Execute();
	}

	private void OnLoadingFailed(ServiceBehaviour failBehaviour)
	{
		ErrorWindow.ShowServiceErrorWindow(failBehaviour);
	}

	private void ProcessLoadedCubeList(ReadOnlyDictionary<CubeTypeID, CubeListData> loadedCubeList)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		DictionaryEnumerator<CubeTypeID, CubeListData> enumerator = loadedCubeList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<CubeTypeID, CubeListData> current = enumerator.get_Current();
				cubeKeysWithoutObsolete.Add(current.Key);
				if (_cubeTypes.ContainsKey(current.Key))
				{
					PersistentCubeData cubeData = _cubeTypes[current.Key].cubeData;
					CubeListData value = current.Value;
					UnpackIntoCubeData(cubeData, value, current.Key);
					cubeData.InitConnections();
					cubeData.InitAttachablePoints();
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		_initialized = true;
	}

	private void UnpackIntoCubeData(PersistentCubeData cubeData, CubeListData value, CubeTypeID cubeKey)
	{
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		cubeData.isIndestructible = value.isIndestructible;
		cubeData.specialCubeKind = value.specialCubeKind;
		cubeData.placementFaces = new CubePlacementFaces();
		cubeData.placementFaces.up = (((value.placementFacesMask >> 0) & 1) != 0);
		cubeData.placementFaces.down = (((value.placementFacesMask >> 1) & 1) != 0);
		cubeData.placementFaces.left = (((value.placementFacesMask >> 2) & 1) != 0);
		cubeData.placementFaces.right = (((value.placementFacesMask >> 3) & 1) != 0);
		cubeData.placementFaces.back = (((value.placementFacesMask >> 4) & 1) != 0);
		cubeData.placementFaces.front = (((value.placementFacesMask >> 5) & 1) != 0);
		if (!string.IsNullOrEmpty(value.descriptionStrKey))
		{
			cubeData.descriptionStrKey = value.descriptionStrKey;
		}
		cubeData.health = value.health;
		cubeData.healthBoost = value.healthBoost;
		cubeData.cpuRating = value.cpuRating;
		cubeData.buildModeVisibility = value.buildVisibility;
		cubeData.greyOutInTutorial = value.greyOutInTutorial;
		cubeData.protoniumCube = value.protoniumCube;
		cubeData.displayStats = (IDictionary<string, object>)(object)value.displayStats;
		cubeData.itemDescriptor = value.itemDescriptor;
		cubeData.itemType = value.itemType;
		cubeData.cubeRanking = value.robotRanking;
		cubeData.variantOf = value.variantOf;
		if (cubeData.itemDescriptor != null && !_itemDescriptorHashCodeToCubeTypeID.ContainsKey(cubeData.itemDescriptor.GenerateKey()))
		{
			_itemDescriptorHashCodeToCubeTypeID.Add(cubeData.itemDescriptor.GenerateKey(), cubeKey);
		}
	}

	public bool IsCubeValid(CubeTypeID type)
	{
		for (int i = 0; i < cubeKeys.Count; i++)
		{
			CubeTypeID cubeTypeID = cubeKeys[i];
			if (cubeTypeID.ID == type.ID)
			{
				return true;
			}
		}
		return false;
	}

	public CubeTypeData CubeTypeDataOf(CubeTypeID type)
	{
		if (!_cubeTypes.ContainsKey(type))
		{
			return null;
		}
		return _cubeTypes[type];
	}

	public void InitializeCubeTypes()
	{
	}

	public BuildVisibility GetBuildModeVisibility(CubeTypeID id)
	{
		return _cubeTypes[id].cubeData.buildModeVisibility;
	}

	public bool GetCubeGreyedOutInTutorial(CubeTypeID id)
	{
		return _cubeTypes[id].cubeData.greyOutInTutorial;
	}

	int ICubeList.GetCubeHealth(uint index)
	{
		CubeTypeID key = new CubeTypeID(index);
		return _cubeTypes[key].cubeData.health;
	}

	public float GetCubeHealthBoost(uint index)
	{
		CubeTypeID key = new CubeTypeID(index);
		return _cubeTypes[key].cubeData.healthBoost;
	}

	public uint GetCubeCPURating(CubeTypeID id)
	{
		if (id.ID == 0)
		{
		}
		if (!_cubeTypes.ContainsKey(id))
		{
			return 1u;
		}
		return _cubeTypes[id].cubeData.cpuRating;
	}

	public bool IsIndestructible(uint id)
	{
		return _cubeTypes[id].cubeData.isIndestructible;
	}

	public ItemDescriptor GetCubeItemDescriptor(CubeTypeID id)
	{
		return _cubeTypes[id].cubeData.itemDescriptor;
	}

	public CubeTypeID GetCubeType(GameObject cubeObject)
	{
		if (_cubeName.TryGetValue(cubeObject.get_name(), out CubeTypeID value))
		{
			return value;
		}
		uint result = 0u;
		Match match = Regex.Match(cubeObject.get_name(), "^[^_]*");
		if (match.Success && uint.TryParse(match.Value, out result))
		{
			return new CubeTypeID(result);
		}
		return CubeTypeID.StandardCubeID;
	}

	public ItemDescriptor GetItemDescriptorFromCube(int itemDescriptorHashCode)
	{
		_itemDescriptorHashCodeToCubeTypeID.TryGetValue(itemDescriptorHashCode, out CubeTypeID value);
		CubeTypeData cubeTypeData = CubeTypeDataOf(value);
		return cubeTypeData.cubeData.itemDescriptor;
	}
}
