using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mothership
{
	internal interface IMachineMap
	{
		event Action<Byte3, MachineCell> OnAddCubeAt;

		event Action<Byte3, MachineCell> OnRemoveCubeAt;

		event Action<Byte3, uint> OnCubeRemovedAt;

		GameObject GetCubeAt(Int3 gridLoc);

		GameObject GetCubeAt(Byte3 gridLoc);

		MachineCell GetCellAt(Int3 gridLoc);

		MachineCell GetCellAt(Byte3 gridLoc);

		void SetCubeAt(Byte3 gridLoc, InstantiatedCube cubeInstance, GameObject goObject);

		bool IsPosValid(Int3 worldPos);

		bool IsPosValid(Byte3 gridLoc);

		bool IsCellTaken(Int3 gridPos);

		bool IsCellTaken(Byte3 gridLoc);

		Int3 FindGridLocFromHit(RaycastHit hit, int iDir);

		void SetGridSizeByMachineType(bool isRobot);

		Byte3 GridSize();

		HashSet<InstantiatedCube> GetAllInstantiatedCubes();

		HashSet<InstantiatedCube> GetRemainingCubes();

		void AddCube(InstantiatedCube cube);

		void RemoveCube(InstantiatedCube cube);

		int GetNumberCubes();

		List<GameObject> GetRecusiveCubesFrom(Byte3 gridPos, bool groupMove, bool includeNonEditable);

		MachineModel BuildMachineLayoutModel();

		void RemoveCellFromMachineMap(Byte3 gridLoc);

		void SilentlyAddCellToMachineMap(Byte3 gridLoc, InstantiatedCube cubeInstance, GameObject goObject);

		Byte3 GetMachineSize();

		IEnumerator GetMachineBounds(Action<Bounds> onSuccess);

		void CalculatOffsetToCentreForModel(MachineModel machineModel, out int offsetX, out int offsetZ);
	}
}
