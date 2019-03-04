using Svelto.DataStructures;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
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

		private Dictionary<uint, MachineCell> _machineMap;

		private FasterList<InstantiatedCube> _cubeInstances;

		private HashSet<InstantiatedCube> _remainingCubes;

		private byte _gridSizeX = 49;

		private byte _gridSizeY = 49;

		private byte _gridSizeZ = 49;

		public MachineMap(int count)
		{
			_machineMap = new Dictionary<uint, MachineCell>(count * 3);
			_cubeInstances = new FasterList<InstantiatedCube>(count);
			_remainingCubes = new HashSet<InstantiatedCube>();
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

		public FasterList<InstantiatedCube> GetAllInstantiatedCubes()
		{
			return _cubeInstances;
		}

		public MachineCell GetCellAt(Int3 gridPos)
		{
			return GetCellAt(gridPos.x, gridPos.y, gridPos.z);
		}

		public MachineCell GetCellAt(Byte3 gridPos)
		{
			uint key = gridPos.GridKey();
			if (_machineMap.TryGetValue(key, out MachineCell value))
			{
				return value;
			}
			return null;
		}

		private MachineCell GetCellAt(int x, int y, int z)
		{
			if (!IsPosValid(x, y, z))
			{
				return null;
			}
			uint key = Byte3.GridKey((byte)x, (byte)y, (byte)z);
			if (_machineMap.TryGetValue(key, out MachineCell value))
			{
				return value;
			}
			return null;
		}

		public GameObject GetCubeAt(Byte3 gridPos)
		{
			return GetCellAt(gridPos)?.gameObject;
		}

		public int GetNumberCubes()
		{
			return _cubeInstances.get_Count();
		}

		void IMachineMap.SetGridSizeByMachineType(bool isRobot)
		{
			if (isRobot)
			{
				_gridSizeX = 65;
				_gridSizeY = 65;
				_gridSizeZ = 65;
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

		public bool IsCellTaken(Byte3 gridPos)
		{
			uint key = gridPos.GridKey();
			return _machineMap.ContainsKey(key);
		}

		public bool IsPosValid(int x, int y, int z)
		{
			if (x < 0 || y < 0 || z < 0 || x >= _gridSizeX || y >= _gridSizeY || z >= _gridSizeZ)
			{
				return false;
			}
			return true;
		}

		public void AddCubeAt(Byte3 gridLoc, InstantiatedCube cubeInstance)
		{
			_cubeInstances.Add(cubeInstance);
			_remainingCubes.Add(cubeInstance);
			MachineCell value = new MachineCell(null, cubeInstance, gridLoc, centre: true);
			_machineMap[gridLoc.GridKey()] = value;
			SetCubeConnections(gridLoc, cubeInstance, null);
		}

		public void UpdateGameObject(InstantiatedCube cubeInstance, GameObject goObject)
		{
			uint key = cubeInstance.gridPos.GridKey();
			MachineCell machineCell = _machineMap[key];
			machineCell.gameObject = goObject;
		}

		private void SetCubeConnections(Byte3 gridLoc, InstantiatedCube cubeInstance, GameObject goObject)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			int extentCubesCount = cubeInstance.persistentCubeData.extentCubesCount;
			if (extentCubesCount > 0)
			{
				List<ConnectionPoint> adjacentCubeLocations = cubeInstance.cubeNodeInstance.GetAdjacentCubeLocations();
				for (int i = 0; i < extentCubesCount; i++)
				{
					ConnectionPoint connectionPoint = adjacentCubeLocations[i];
					Byte3 gridLoc2 = gridLoc + new Byte3(CubeData.IndexToQuat(cubeInstance.rotationIndex) * connectionPoint.offset);
					AddCubeExtentAt(gridLoc2, cubeInstance, goObject);
				}
			}
		}

		public void AddCubeExtentAt(Byte3 gridLoc, InstantiatedCube cubeInstance, GameObject g)
		{
			uint key = Byte3.GridKey(gridLoc.x, gridLoc.y, gridLoc.z);
			if (!_machineMap.ContainsKey(key))
			{
				_machineMap[key] = new MachineCell(g, cubeInstance, gridLoc, centre: false);
			}
		}
	}
}
