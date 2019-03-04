using Svelto.DataStructures;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal interface IMachineMap
	{
		GameObject GetCubeAt(Byte3 gridLoc);

		MachineCell GetCellAt(Int3 gridLoc);

		MachineCell GetCellAt(Byte3 gridLoc);

		void AddCubeAt(Byte3 gridLoc, InstantiatedCube cubeInstance);

		void AddCubeExtentAt(Byte3 gridLoc, InstantiatedCube cubeInstance, GameObject go);

		void UpdateGameObject(InstantiatedCube cubeInstance, GameObject goObject);

		void SetGridSizeByMachineType(bool isRobot);

		bool IsPosValid(int x, int y, int z);

		bool IsCellTaken(Byte3 gridLoc);

		Byte3 GridSize();

		FasterList<InstantiatedCube> GetAllInstantiatedCubes();

		HashSet<InstantiatedCube> GetRemainingCubes();

		void AddCube(InstantiatedCube cube);

		void RemoveCube(InstantiatedCube cube);

		int GetNumberCubes();
	}
}
