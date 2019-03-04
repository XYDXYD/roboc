using Mothership;
using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;

internal class MachineBuilder : IMachineBuilder
{
	private FasterList<InstantiatedCube> _cubesToDelete = new FasterList<InstantiatedCube>();

	private Action _buildHistoryAction;

	[Inject]
	internal ICubeFactory cubeFactory
	{
		private get;
		set;
	}

	[Inject]
	internal ICubeHolder cubeHolder
	{
		get;
		private set;
	}

	[Inject]
	internal ICubeInventory cubeInventory
	{
		private get;
		set;
	}

	[Inject]
	internal ICubeList cubeTypeInventory
	{
		private get;
		set;
	}

	[Inject]
	internal IMachineMap machineMap
	{
		private get;
		set;
	}

	[Inject]
	internal BuildHistoryManager buildHistoryManager
	{
		private get;
		set;
	}

	[Inject]
	internal GameObjectPool objectPool
	{
		get;
		private set;
	}

	[Inject]
	internal MachineMover machineMover
	{
		private get;
		set;
	}

	[Inject]
	internal MirrorMode mirrorMode
	{
		private get;
		set;
	}

	public event Action<InstantiatedCube> OnPlaceCube = delegate
	{
	};

	public event Action<InstantiatedCube> OnDeleteCube = delegate
	{
	};

	public event Action OnAllCubesRemoved = delegate
	{
	};

	public bool CheckIfCubeSideIsValid(Int3 adjacentGridPos, Vector3 cubeUp, CubeTypeID cubeID)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return CanPlaceCubeNextToAnotherCube(adjacentGridPos, cubeUp) && CanPlaceCubeOnHitSide(cubeUp, cubeID);
	}

	public bool CheckIfCanPlaceCube(Int3 cubeGridPos, ICubeCaster cubeCaster)
	{
		Int3 offGridPoint;
		return CanPlaceCube(cubeGridPos, cubeCaster) || CanPlaceCubeIfMachineMove(cubeGridPos, out offGridPoint, cubeCaster);
	}

	public bool CanPlaceCubeOnHitSide(Vector3 placement_normal, CubeTypeID cubeID)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		PersistentCubeData cubeData = cubeTypeInventory.CubeTypeDataOf(cubeID).cubeData;
		return cubeData.IsPlacementFaceValid(placement_normal);
	}

	public bool IsInsideGrid(RaycastHit _rcHit, CubeTypeID cubeID)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Int3 worldPos = machineMap.FindGridLocFromHit(_rcHit, 1);
		return machineMap.IsPosValid(worldPos);
	}

	public GameObject TryPlaceCubeWithRotation(Int3 gridPos, Int3 adjacentGridPos, Vector3 cubeUp, CubeTypeID typeId, int localRotation, ICubeCaster cubeCaster, Quaternion placementRotation)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		if (CanPlaceCube(gridPos, cubeCaster))
		{
			return PlaceCube(gridPos, cubeUp, typeId, localRotation, TargetType.Player, placementRotation);
		}
		if (CanPlaceCubeIfMachineMove(gridPos, out Int3 offGridPoint, cubeCaster))
		{
			Int3 @int = ComputeDisplacement(offGridPoint);
			HashSet<InstantiatedCube> allInstantiatedCubes = machineMap.GetAllInstantiatedCubes();
			if (machineMover.CanMoveMachineWhithNoCubesDestroyed(allInstantiatedCubes, @int))
			{
				GameObject cubeAt = machineMap.GetCubeAt(adjacentGridPos);
				if (cubeAt != null)
				{
					machineMover.MoveCubesToValidCellOrDestroy(allInstantiatedCubes, @int);
					Int3 reverseGridDisplacement = new Int3(-1 * @int.x, -1 * @int.y, -1 * @int.z);
					Action action = delegate
					{
						HashSet<InstantiatedCube> allInstantiatedCubes2 = machineMap.GetAllInstantiatedCubes();
						machineMover.MoveCubesToValidCellOrDestroy(allInstantiatedCubes2, reverseGridDisplacement);
					};
					buildHistoryManager.StoreUndoBuildAction(action);
				}
				gridPos += @int;
				return PlaceCube(gridPos, cubeUp, typeId, localRotation, TargetType.Player, placementRotation);
			}
		}
		return null;
	}

	public GameObject TryPlaceCubeOnSurface(Int3 cubeGridPos, Int3 adjacentGridPos, Vector3 cubeUp, ICubeCaster cubeCaster, Quaternion cubeRotation)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!CheckIfCubeSideIsValid(adjacentGridPos, cubeUp, cubeHolder.selectedCubeID))
		{
			return null;
		}
		return TryPlaceCubeWithRotation(cubeGridPos, adjacentGridPos, cubeUp, cubeHolder.selectedCubeID, cubeHolder.currentRotation, cubeCaster, cubeRotation);
	}

	public GameObject FindCubeHit(RaycastHit hit)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Int3 gridLoc = machineMap.FindGridLocFromHit(hit, -1);
		return machineMap.GetCubeAt(gridLoc);
	}

	public void RemoveCube(InstantiatedCube cubeInstance)
	{
		CubeTypeID cubeTypeID = SilentRemoveCube(cubeInstance);
		this.OnDeleteCube(cubeInstance);
	}

	public void RemoveAllCubesGarage()
	{
		List<InstantiatedCube> list = new List<InstantiatedCube>(machineMap.GetAllInstantiatedCubes());
		for (int i = 0; i < list.Count; i++)
		{
			InstantiatedCube cubeInstance = list[i];
			SilentRemoveCube(cubeInstance);
		}
		if (list.Count > 0)
		{
			this.OnAllCubesRemoved();
		}
	}

	public void RemoveAllCubes()
	{
		List<InstantiatedCube> list = new List<InstantiatedCube>(machineMap.GetAllInstantiatedCubes());
		foreach (InstantiatedCube item in list)
		{
			RemoveCube(item);
		}
	}

	public void RemoveAllCubesByItemDescriptor(int itemDescriptor)
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		_cubesToDelete.FastClear();
		HashSet<InstantiatedCube> allInstantiatedCubes = machineMap.GetAllInstantiatedCubes();
		HashSet<InstantiatedCube>.Enumerator enumerator = allInstantiatedCubes.GetEnumerator();
		while (enumerator.MoveNext())
		{
			InstantiatedCube current = enumerator.Current;
			PersistentCubeData persistentCubeData = current.persistentCubeData;
			if (persistentCubeData.itemDescriptor != null && persistentCubeData.itemDescriptor.isActivable && persistentCubeData.itemDescriptor.GenerateKey() == itemDescriptor)
			{
				_cubesToDelete.Add(current);
			}
		}
		FasterListEnumerator<InstantiatedCube> enumerator2 = _cubesToDelete.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			RemoveCube(enumerator2.get_Current());
		}
	}

	public void RemoveObsoleteCubes(MachineModel model)
	{
		for (int i = 0; i < model.DTO.Count; i++)
		{
			CubeData cubeData = model.DTO[i];
			if (!cubeTypeInventory.IsCubeValid(cubeData.iID))
			{
				model.DTO.RemoveAt(i--);
			}
		}
	}

	public GameObject CreateCube(Int3 gridPos, CubeTypeID cubeTypeID, Quaternion alreadyComputedRotation, TargetType targetType, byte paletteIndex, PaletteColor paletteColor)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return CreateCube((Byte3)gridPos, cubeTypeID, alreadyComputedRotation, targetType, paletteIndex, paletteColor);
	}

	private GameObject PlaceCube(Int3 gridPos, Vector3 cubeUp, CubeTypeID typeId, int rotationAmount, TargetType targetType, Quaternion cubeRotation)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (!cubeTypeInventory.IsCubeValid(typeId))
		{
			return null;
		}
		Quaternion finalRotation = GetFinalRotation(cubeRotation, rotationAmount);
		GameObject val = CreateCube((Byte3)gridPos, typeId, finalRotation, targetType, cubeHolder.currentPaletteId, cubeHolder.currentColor);
		if (val != null)
		{
			_buildHistoryAction = delegate
			{
				MachineCell cellAt = machineMap.GetCellAt((Byte3)gridPos);
				if (cellAt != null)
				{
					InstantiatedCube info = cellAt.info;
					RemoveCube(info);
				}
			};
			buildHistoryManager.StoreUndoBuildAction(_buildHistoryAction);
		}
		return val;
	}

	private static Quaternion GetFinalRotation(Quaternion placementRotation, int localRotation)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = placementRotation * Quaternion.AngleAxis((float)localRotation, Vector3.get_up());
		Vector3 eulerAngles = val.get_eulerAngles();
		eulerAngles.x = Mathf.RoundToInt(eulerAngles.x / 90f);
		eulerAngles.y = Mathf.RoundToInt(eulerAngles.y / 90f);
		eulerAngles.z = Mathf.RoundToInt(eulerAngles.z / 90f);
		return Quaternion.Euler(eulerAngles.x * 90f, eulerAngles.y * 90f, eulerAngles.z * 90f);
	}

	private GameObject CreateCube(Byte3 gridPos, CubeTypeID cubeTypeId, Quaternion alreadyComputedRotation, TargetType targetType, byte paletteIndex, PaletteColor paletteColor)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = cubeFactory.BuildCube(cubeTypeId, GridScaleUtility.GridToWorld(gridPos, targetType), alreadyComputedRotation, targetType);
		Vector3 val2 = val.get_transform().get_position();
		val2 *= 10f;
		val2.Set((float)Mathf.RoundToInt(val2.x), (float)Mathf.RoundToInt(val2.y), (float)Mathf.RoundToInt(val2.z));
		val.get_transform().set_position(new Vector3(val2.x / 10f, val2.y / 10f, val2.z / 10f));
		CubeNodeInstance cubeNodeInstance = new CubeNodeInstance();
		InstantiatedCube instantiatedCube2 = cubeNodeInstance.instantiatedCube = new InstantiatedCube(val.GetComponent<CubeInstance>(), cubeNodeInstance, cubeTypeInventory.CubeTypeDataOf(cubeTypeId).cubeData, gridPos, CubeData.QuatToIndex(val.get_transform().get_rotation()));
		cubeNodeInstance.isDestroyed = false;
		int num = instantiatedCube2.health = (instantiatedCube2.initialTotalHealth = cubeTypeInventory.GetCubeHealth(cubeTypeId.ID));
		machineMap.SetCubeAt(gridPos, instantiatedCube2, val);
		instantiatedCube2.paletteColor = paletteColor;
		instantiatedCube2.paletteIndex = paletteIndex;
		this.OnPlaceCube(instantiatedCube2);
		return val;
	}

	private Int3 ComputeDisplacement(Int3 pos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		Vector3 zero = Vector3.get_zero();
		Byte3 @byte = machineMap.GridSize();
		if ((float)pos.x < 0f)
		{
			zero.x = (float)pos.x * -1f;
		}
		else if (pos.x >= @byte.x)
		{
			zero.x = (float)(int)@byte.x - ((float)pos.x + 1f);
		}
		if ((float)pos.y < 0f)
		{
			zero.y = (float)pos.y * -1f;
		}
		else if (pos.y >= @byte.y)
		{
			zero.y = (float)(int)@byte.y - ((float)pos.y + 1f);
		}
		if ((float)pos.z < 0f)
		{
			zero.z = (float)pos.z * -1f;
		}
		else if (pos.z >= @byte.z)
		{
			zero.z = (float)(int)@byte.z - ((float)pos.z + 1f);
		}
		return new Int3(zero);
	}

	private CubeTypeID SilentRemoveCube(InstantiatedCube cubeInstance)
	{
		GameObject cubeAt = machineMap.GetCubeAt(cubeInstance.gridPos);
		machineMap.SetCubeAt(cubeInstance.gridPos, null, null);
		objectPool.Recycle(cubeAt, cubeAt.get_name());
		cubeAt.SetActive(false);
		return cubeInstance.persistentCubeData.cubeType;
	}

	private bool CanPlaceCubeNextToAnotherCube(Int3 adjacentCubePos, Vector3 hitNormal)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		if (AdjacentCubeGridPosIsOnGround(adjacentCubePos))
		{
			return true;
		}
		MachineCell cellAt = machineMap.GetCellAt(adjacentCubePos);
		if (cellAt == null)
		{
			return false;
		}
		Transform transform = cellAt.gameObject.get_transform();
		if (transform != null)
		{
			Quaternion val = CubeData.IndexToQuat(cellAt.info.rotationIndex);
			Vector3 localOffset = Quaternion.Inverse(val) * (adjacentCubePos - new Int3(cellAt.pos)).ToVector3();
			return cellAt.info.persistentCubeData.IsDirectionSelectable(transform.get_rotation(), hitNormal, localOffset);
		}
		return true;
	}

	private bool AdjacentCubeGridPosIsOnGround(Int3 hitCubePos)
	{
		return hitCubePos.y < 0;
	}

	private void FindCubeOnTheFloor(RaycastHit rcHit, ref GameObject cube)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Int3 gridLoc = machineMap.FindGridLocFromHit(rcHit, -1);
		cube = machineMap.GetCubeAt(gridLoc);
	}

	private bool CanPlaceCube(Int3 gridPos, ICubeCaster cubeCaster)
	{
		return machineMap.IsPosValid(gridPos) && !machineMap.IsCellTaken(gridPos) && !cubeCaster.ghostIntersectsFloor;
	}

	private bool CanPlaceCubeIfMachineMove(Int3 gridPos, out Int3 offGridPoint, ICubeCaster cubeCaster)
	{
		offGridPoint = default(Int3);
		if (machineMap.IsPosValid(gridPos) && !machineMap.IsCellTaken(gridPos))
		{
			offGridPoint = cubeCaster.displacement;
			return true;
		}
		return false;
	}
}
