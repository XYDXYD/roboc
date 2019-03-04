using Svelto.IoC;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public sealed class CubesTypeList : MonoBehaviour, IInitialize
{
	public List<CubeTypeData> cubeTypes;

	private CubesTypeListOverrides _overridesList;

	private const string OVERRIDES_LIST = "CubesTypeListOverrides_Infinity";

	public Dictionary<uint, CubeTypeData> cubeTypeDic
	{
		get;
		private set;
	}

	[Inject]
	internal ICubeList cubeList
	{
		private get;
		set;
	}

	public CubesTypeList()
		: this()
	{
	}

	public void InitCubeLists()
	{
		InitialiseOverridesListIfNotAlready();
		cubeTypeDic = new Dictionary<uint, CubeTypeData>();
		foreach (CubeTypeData cubeType in cubeTypes)
		{
			if (!uint.TryParse(cubeType.itemCode, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint result))
			{
				throw new Exception("CubeTypeInfo - invalid cube hash");
			}
			cubeType.itemCodeValue = result;
			cubeType.cubeData.cubeType = new CubeTypeID(result);
			if (!cubeTypeDic.ContainsKey(cubeType.itemCodeValue))
			{
				GameObject go = null;
				if (_overridesList.GetCube(cubeType.itemCode, out go))
				{
					Debug.Log((object)("overriding " + cubeType.nameStrKey + " with platform variant"));
					cubeType.prefab = go;
				}
				cubeTypeDic.Add(cubeType.itemCodeValue, cubeType);
			}
		}
	}

	void IInitialize.OnDependenciesInjected()
	{
		InitCubeLists();
		cubeList.InitCubeCatalog(cubeTypeDic);
	}

	private void InitialiseOverridesListIfNotAlready()
	{
		if (_overridesList == null)
		{
			_overridesList = (Resources.Load("CubesTypeListOverrides_Infinity") as CubesTypeListOverrides);
		}
		if (_overridesList == null)
		{
			Debug.LogError((object)"could not load overrides list for this platform! Maybe the file is missing?");
		}
	}
}
