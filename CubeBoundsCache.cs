using Simulation.Hardware.Weapons;
using Svelto.IoC;
using System.Collections.Generic;
using UnityEngine;

internal sealed class CubeBoundsCache
{
	private Dictionary<CubeTypeID, CubeExtentBounds> _cubeBounds = new Dictionary<CubeTypeID, CubeExtentBounds>();

	[Inject]
	internal ICubeList _cubeList
	{
		private get;
		set;
	}

	[Inject]
	internal ICubeFactory _cubeFactory
	{
		private get;
		set;
	}

	[Inject]
	internal GameObjectPool _pool
	{
		private get;
		set;
	}

	public CubeExtentBounds GetCubeBounds(InstantiatedCube cube)
	{
		return GetCubeBounds(cube.persistentCubeData.cubeType, cube.rotationIndex);
	}

	public CubeExtentBounds GetCubeBounds(CubeTypeID cubeType, int rotationIndex)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return GetCubeBounds(cubeType, CubeData.IndexToQuat(rotationIndex));
	}

	public CubeExtentBounds GetCubeBounds(CubeTypeID cubeType, Quaternion rotation)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (!_cubeBounds.ContainsKey(cubeType))
		{
			CubeExtentBounds value = ComputeCubeBounds(cubeType);
			_cubeBounds[cubeType] = value;
		}
		CubeExtentBounds cubeExtentBounds = new CubeExtentBounds(_cubeBounds[cubeType]);
		cubeExtentBounds.Rotate(rotation);
		return cubeExtentBounds;
	}

	private static bool IsShapeCollider(Collider c)
	{
		return c.get_gameObject().get_layer() == GameLayers.BUILD_COLLISION || c.get_gameObject().get_layer() == GameLayers.BUILDCOLLISION_UNVERIFIED;
	}

	private CubeExtentBounds ComputeCubeBounds(CubeTypeID id)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		CubeTypeData cubeTypeData = _cubeList.CubeTypeDataOf(id);
		TargetType targetType = TargetType.Player;
		ICubeFactory cubeFactory = _cubeFactory;
		Vector3 one = Vector3.get_one();
		MachineScale.MachineScaleData machineScaleData = MachineScale.Scales[targetType];
		GameObject val = cubeFactory.BuildCube(id, one * machineScaleData.halfCell, Quaternion.get_identity(), targetType);
		Collider[] componentsInChildren = val.GetComponentsInChildren<Collider>();
		CubeExtentBounds cubeExtentBounds = new CubeExtentBounds();
		Collider[] array = componentsInChildren;
		foreach (Collider val2 in array)
		{
			if (IsShapeCollider(val2))
			{
				GridScaleUtility.MinMaxBoundsToGrid(val2.get_bounds(), targetType, out Int3 min, out Int3 max);
				for (int j = 0; j < 3; j++)
				{
					cubeExtentBounds.lowerBound[j] = Mathf.Min(cubeExtentBounds.lowerBound[j], min[j]);
					cubeExtentBounds.upperBound[j] = Mathf.Max(cubeExtentBounds.upperBound[j], max[j]);
				}
			}
		}
		_pool.Recycle(val, val.get_name());
		val.SetActive(false);
		return cubeExtentBounds;
	}
}
