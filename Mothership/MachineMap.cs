using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mothership
{
	internal sealed class MachineMap : IMachineMap
	{
		private const byte DEFAULT_GRID_SIZE_X = byte.MaxValue;

		private const byte DEFAULT_GRID_SIZE_Y = byte.MaxValue;

		private const byte DEFAULT_GRID_SIZE_Z = byte.MaxValue;

		public const byte STANDARD_BAY_SIZE_X = 49;

		public const byte STANDARD_BAY_SIZE_Y = 49;

		public const byte STANDARD_BAY_SIZE_Z = 49;

		public const byte SIMULATION_LOWER_PADDING_X = 8;

		public const byte SIMULATION_LOWER_PADDING_Y = 8;

		public const byte SIMULATION_LOWER_PADDING_Z = 8;

		private const byte SIMULATION_UPPER_PADDING_X = 8;

		private const byte SIMULATION_UPPER_PADDING_Y = 8;

		private const byte SIMULATION_UPPER_PADDING_Z = 8;

		private Dictionary<uint, MachineCell> _machineMap = new Dictionary<uint, MachineCell>();

		private Dictionary<uint, MachineCell> _oobMachineMap = new Dictionary<uint, MachineCell>();

		private HashSet<InstantiatedCube> _cubeInstances = new HashSet<InstantiatedCube>();

		private byte _gridSizeX = byte.MaxValue;

		private byte _gridSizeY = byte.MaxValue;

		private byte _gridSizeZ = byte.MaxValue;

		private HashSet<InstantiatedCube> _remainingCubes = new HashSet<InstantiatedCube>();

		public event Action<Byte3, MachineCell> OnAddCubeAt = delegate
		{
		};

		public event Action<Byte3, MachineCell> OnRemoveCubeAt = delegate
		{
		};

		public event Action<Byte3, uint> OnCubeRemovedAt = delegate
		{
		};

		void IMachineMap.SetGridSizeByMachineType(bool isRobot)
		{
			if (isRobot)
			{
				_gridSizeX = 49;
				_gridSizeY = 49;
				_gridSizeZ = 49;
			}
			else
			{
				_gridSizeX = byte.MaxValue;
				_gridSizeY = byte.MaxValue;
				_gridSizeZ = byte.MaxValue;
			}
		}

		public Byte3 GridSize()
		{
			return new Byte3(_gridSizeX, _gridSizeY, _gridSizeZ);
		}

		public void SetCubeAt(Byte3 gridLoc, InstantiatedCube cubeInstance, GameObject goObject)
		{
			if (goObject == null)
			{
				ClearCubeAt(gridLoc);
				return;
			}
			MachineCell cell = SetCubeConnections(ref gridLoc, cubeInstance, goObject);
			SetCubeAt(gridLoc, cell);
		}

		private MachineCell SetCubeConnections(ref Byte3 gridLoc, InstantiatedCube cubeInstance, GameObject goObject)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			MachineCell machineCell = new MachineCell(goObject, cubeInstance, gridLoc, centre: true);
			List<ConnectionPoint> adjacentCubeLocations = cubeInstance.cubeNodeInstance.GetAdjacentCubeLocations();
			for (int i = 0; i < adjacentCubeLocations.Count; i++)
			{
				ConnectionPoint connectionPoint = adjacentCubeLocations[i];
				if (connectionPoint.offset.get_sqrMagnitude() > 0.1f)
				{
					Byte3 @byte = gridLoc + new Byte3(CubeData.IndexToQuat(cubeInstance.rotationIndex) * connectionPoint.offset);
					SilentSetCubeAt(@byte, new MachineCell(goObject, cubeInstance, gridLoc, centre: false));
					machineCell.info.boundsOccupiedCells.Add(@byte);
				}
			}
			return machineCell;
		}

		private void SetCubeAt(Byte3 gridLoc, MachineCell cell)
		{
			gridLoc = SilentSetCubeAt(gridLoc, cell);
			this.OnAddCubeAt(gridLoc, cell);
		}

		private Byte3 SilentSetCubeAt(Byte3 gridLoc, MachineCell cell)
		{
			uint key = Byte3.GridKey(gridLoc.x, gridLoc.y, gridLoc.z);
			bool flag = _machineMap.ContainsKey(key);
			if (!flag || (flag && cell.centreCell))
			{
				_machineMap[key] = cell;
			}
			_cubeInstances.Add(cell.info);
			return gridLoc;
		}

		public void SilentlyAddCellToMachineMap(Byte3 gridLoc, InstantiatedCube cubeInstance, GameObject goObject)
		{
			MachineCell cell = SetCubeConnections(ref gridLoc, cubeInstance, goObject);
			SilentSetCubeAt(gridLoc, cell);
		}

		private void ClearCubeAt(Byte3 gridLoc)
		{
			uint key = Byte3.GridKey(gridLoc.x, gridLoc.y, gridLoc.z);
			MachineCell machineCell = _machineMap[key];
			uint iD = machineCell.info.persistentCubeData.cubeType.ID;
			if (machineCell.centreCell)
			{
				this.OnRemoveCubeAt(gridLoc, machineCell);
			}
			ActuallyRemoveCellFromMachineMap(machineCell);
			if (machineCell.centreCell)
			{
				this.OnCubeRemovedAt(gridLoc, iD);
			}
		}

		public void RemoveCellFromMachineMap(Byte3 gridLoc)
		{
			uint key = gridLoc.GridKey();
			if (_machineMap.ContainsKey(key))
			{
				MachineCell cellToRemove = _machineMap[key];
				ActuallyRemoveCellFromMachineMap(cellToRemove);
			}
		}

		private void ActuallyRemoveCellFromMachineMap(MachineCell cellToRemove)
		{
			uint iD = cellToRemove.info.persistentCubeData.cubeType.ID;
			_cubeInstances.Remove(cellToRemove.info);
			for (int i = 0; i < cellToRemove.info.boundsOccupiedCells.Count; i++)
			{
				uint key = cellToRemove.info.boundsOccupiedCells[i].GridKey();
				if (_machineMap.TryGetValue(key, out MachineCell value) && !value.IsCentreCell())
				{
					_machineMap.Remove(key);
				}
			}
			cellToRemove.info.boundsOccupiedCells.Clear();
			Byte3 gridPos = cellToRemove.info.gridPos;
			_machineMap.Remove(gridPos.GridKey());
		}

		public MachineCell GetCellAt(Byte3 gridPos)
		{
			return GetCellAt(gridPos.x, gridPos.y, gridPos.z);
		}

		public MachineCell GetCellAt(Int3 gridPos)
		{
			return GetCellAt(gridPos.x, gridPos.y, gridPos.z);
		}

		public MachineCell GetCellAt(int x, int y, int z)
		{
			if (!IsPosValid(x, y, z))
			{
				return null;
			}
			uint key = Byte3.GridKey((byte)x, (byte)y, (byte)z);
			if (_machineMap.ContainsKey(key))
			{
				return _machineMap[key];
			}
			return null;
		}

		public MachineCell GetCellWithObject(GameObject obj)
		{
			foreach (MachineCell value in _machineMap.Values)
			{
				if (value.gameObject != null && value.gameObject == obj)
				{
					return value;
				}
			}
			return null;
		}

		public GameObject GetCubeAt(Byte3 gridPos)
		{
			return GetCubeAt(gridPos.x, gridPos.y, gridPos.z);
		}

		public GameObject GetCubeAt(Int3 gridPos)
		{
			return GetCubeAt(gridPos.x, gridPos.y, gridPos.z);
		}

		public GameObject GetCubeAt(int x, int y, int z)
		{
			return GetCellAt(x, y, z)?.gameObject;
		}

		public bool IsCellTaken(Byte3 gridPos)
		{
			return GetCellAt(gridPos.x, gridPos.y, gridPos.z) != null;
		}

		public bool IsCellTaken(Int3 gridPos)
		{
			return GetCellAt(gridPos.x, gridPos.y, gridPos.z) != null;
		}

		public bool IsPosValid(Int3 gridLoc)
		{
			return IsPosValid(gridLoc.x, gridLoc.y, gridLoc.z);
		}

		public bool IsPosValid(Byte3 gridLoc)
		{
			return IsPosValid(gridLoc.x, gridLoc.y, gridLoc.z);
		}

		public bool IsPosValid(int x, int y, int z)
		{
			if (x < 0 || y < 0 || z < 0 || x >= _gridSizeX || y >= _gridSizeY || z >= _gridSizeZ)
			{
				return false;
			}
			return true;
		}

		public Int3 FindGridLocFromHit(RaycastHit hit, int iDir)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			Vector3 point = hit.get_point();
			Vector3 normal = hit.get_normal();
			float num = iDir;
			MachineScale.MachineScaleData machineScaleData = MachineScale.Scales[TargetType.Player];
			return GridScaleUtility.WorldToGrid(point + normal * (num * machineScaleData.halfCell), TargetType.Player);
		}

		public int GetNumberCubes()
		{
			return _cubeInstances.Count;
		}

		public HashSet<InstantiatedCube> GetAllInstantiatedCubes()
		{
			return _cubeInstances;
		}

		public List<GameObject> GetRecusiveCubesFrom(Byte3 gridPos, bool groupMove, bool includeNonEditable)
		{
			List<GameObject> cubes = new List<GameObject>();
			GameObject cubeAt = GetCubeAt(gridPos);
			if (cubeAt != null)
			{
				cubes.Add(cubeAt);
				if (groupMove)
				{
					RecursiveGetConnectedCubes(gridPos, ref cubes, includeNonEditable);
				}
			}
			return cubes;
		}

		private void RecursiveGetConnectedCubes(Byte3 gridPos, ref List<GameObject> cubes, bool includeNonEditable)
		{
			for (int i = 0; i < CubeFaceExtensions.NumberOfFaces(); i++)
			{
				Byte3 gridPos2 = gridPos + MachineUtility.GetDirection((CubeFace)i);
				GameObject cubeAt = GetCubeAt(gridPos2);
				if (cubeAt != null && !cubes.Contains(cubeAt))
				{
					cubes.Add(cubeAt);
					RecursiveGetConnectedCubes(gridPos2, ref cubes, includeNonEditable);
				}
			}
		}

		private void CleanMachineLayout()
		{
			_machineMap.Clear();
			_cubeInstances.Clear();
		}

		public MachineModel BuildMachineLayoutModel()
		{
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			MachineModel machineModel = new MachineModel();
			foreach (MachineCell value in _machineMap.Values)
			{
				if (value.centreCell)
				{
					InstantiatedCube info = value.info;
					GameObject gameObject = value.gameObject;
					CubeData cubeData = new CubeData();
					cubeData.iID = (uint)info.persistentCubeData.cubeType;
					cubeData.gridLocation = value.pos;
					cubeData.rotationIndex = (byte)CubeData.QuatToIndex(gameObject.get_transform().get_rotation());
					cubeData.paletteIndex = info.paletteIndex;
					machineModel.DTO.Add(cubeData);
				}
			}
			return machineModel;
		}

		public Byte3 GetMachineSize()
		{
			Byte3 result = default(Byte3);
			Dictionary<uint, MachineCell>.Enumerator enumerator = _machineMap.GetEnumerator();
			if (enumerator.MoveNext())
			{
				Byte3 pos = enumerator.Current.Value.pos;
				byte b = pos.x;
				byte b2 = pos.y;
				byte b3 = pos.z;
				byte b4 = b;
				byte b5 = b2;
				byte b6 = b3;
				while (enumerator.MoveNext())
				{
					pos = enumerator.Current.Value.pos;
					b = Math.Min(b, pos.x);
					b2 = Math.Min(b2, pos.y);
					b3 = Math.Min(b3, pos.z);
					b4 = Math.Max(b4, pos.x);
					b5 = Math.Max(b5, pos.y);
					b6 = Math.Max(b6, pos.z);
				}
				result.x = (byte)(b4 - b);
				result.y = (byte)(b5 - b2);
				result.z = (byte)(b6 - b3);
			}
			return result;
		}

		public IEnumerator GetMachineBounds(Action<Bounds> onSuccess)
		{
			bool boundValid = false;
			Bounds bounds = default(Bounds);
			int count = 0;
			Dictionary<uint, MachineCell> copyMap = new Dictionary<uint, MachineCell>(_machineMap);
			Dictionary<uint, MachineCell>.Enumerator enumerator = copyMap.GetEnumerator();
			while (enumerator.MoveNext())
			{
				GameObject goCube = enumerator.Current.Value.gameObject;
				Collider[] colliders = goCube.GetComponentsInChildren<Collider>();
				foreach (Collider val in colliders)
				{
					if (boundValid)
					{
						bounds.Encapsulate(val.get_bounds());
						continue;
					}
					Bounds bounds2 = val.get_bounds();
					bounds.set_center(bounds2.get_center());
					Bounds bounds3 = val.get_bounds();
					bounds.set_extents(bounds3.get_extents());
					Bounds bounds4 = val.get_bounds();
					bounds.set_max(bounds4.get_max());
					Bounds bounds5 = val.get_bounds();
					bounds.set_min(bounds5.get_min());
					Bounds bounds6 = val.get_bounds();
					bounds.set_size(bounds6.get_size());
					boundValid = true;
				}
				count++;
				if (count >= 30)
				{
					count = 0;
					yield return null;
				}
			}
			if (!boundValid)
			{
				Byte3 pos = new Byte3(_gridSizeX, (byte)((int)_gridSizeY / 2), _gridSizeZ);
				bounds.set_center(GridScaleUtility.GridToWorld(pos, TargetType.Player));
			}
			Vector3 size = bounds.get_size();
			if (size.x < 1f)
			{
				bounds.Expand(GridScaleUtility.GridToWorld(new Byte3(1, 0, 0), TargetType.Player));
			}
			Vector3 size2 = bounds.get_size();
			if (size2.y < 1f)
			{
				bounds.Expand(GridScaleUtility.GridToWorld(new Byte3(0, 1, 0), TargetType.Player));
			}
			Vector3 size3 = bounds.get_size();
			if (size3.z < 1f)
			{
				bounds.Expand(GridScaleUtility.GridToWorld(new Byte3(0, 0, 1), TargetType.Player));
			}
			SafeEvent.SafeRaise<Bounds>(onSuccess, bounds);
			yield return bounds;
		}

		public void CalculatOffsetToCentreForModel(MachineModel machineModel, out int offsetX, out int offsetZ)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			offsetX = 0;
			offsetZ = 0;
			Byte3 c = new Byte3(byte.MaxValue, byte.MaxValue, byte.MaxValue);
			Byte3 c2 = new Byte3(0, 0, 0);
			FasterReadOnlyList<CubeData> cubes = machineModel.DTO.cubes;
			FasterListEnumerator<CubeData> enumerator = cubes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					CubeData current = enumerator.get_Current();
					if (current.gridLocation.x < c.x)
					{
						c.x = current.gridLocation.x;
					}
					if (current.gridLocation.y < c.y)
					{
						c.y = current.gridLocation.y;
					}
					if (current.gridLocation.z < c.z)
					{
						c.z = current.gridLocation.z;
					}
					if (current.gridLocation.x > c2.x)
					{
						c2.x = current.gridLocation.x;
					}
					if (current.gridLocation.y > c2.y)
					{
						c2.y = current.gridLocation.y;
					}
					if (current.gridLocation.z > c2.z)
					{
						c2.z = current.gridLocation.z;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			if (cubes.get_Count() == 0)
			{
				c = new Byte3(0, 0, 0);
				c2 = new Byte3(0, 0, 0);
			}
			Byte3 @byte = c2 - c;
			float num = (float)(int)@byte.x * 0.5f;
			float num2 = (float)(int)@byte.z * 0.5f;
			float num3 = (float)(int)_gridSizeX * 0.5f;
			float num4 = (float)(int)_gridSizeZ * 0.5f;
			float num5 = (float)(int)c.x + num + 0.5f;
			float num6 = (float)(int)c.z + num2 + 0.5f;
			int num7 = 0;
			int num8 = 0;
			if (num5 != num3 || num6 != num4)
			{
				num7 = (int)(num4 - num6);
				num8 = (int)(num3 - num5);
			}
			offsetX = num8;
			offsetZ = num7;
		}

		public HashSet<InstantiatedCube> GetRemainingCubes()
		{
			return _remainingCubes;
		}

		public void AddCube(InstantiatedCube cube)
		{
			_remainingCubes.Add(cube);
		}

		public void RemoveCube(InstantiatedCube cube)
		{
			_remainingCubes.Remove(cube);
		}
	}
}
