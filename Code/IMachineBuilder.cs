using Simulation.Hardware.Weapons;
using System;
using UnityEngine;

internal interface IMachineBuilder
{
	event Action<InstantiatedCube> OnPlaceCube;

	event Action<InstantiatedCube> OnDeleteCube;

	event Action OnAllCubesRemoved;

	bool IsInsideGrid(RaycastHit _rcHit, CubeTypeID cubeID);

	bool CheckIfCubeSideIsValid(Int3 adjacentGridPos, Vector3 cubeUp, CubeTypeID cubeID);

	bool CheckIfCanPlaceCube(Int3 cubeGridPos, ICubeCaster cubeCaster);

	bool CanPlaceCubeOnHitSide(Vector3 sideNormal, CubeTypeID cubeID);

	GameObject TryPlaceCubeWithRotation(Int3 gridPos, Int3 adjacentGridPos, Vector3 cubeUp, CubeTypeID typeId, int rotationAmount, ICubeCaster cubeCaster, Quaternion cubeRotation);

	GameObject TryPlaceCubeOnSurface(Int3 gridPos, Int3 adjacentGridPos, Vector3 cubeUp, ICubeCaster cubeCaster, Quaternion cubeRotation);

	GameObject CreateCube(Int3 gridPos, CubeTypeID cubeTypeID, Quaternion rotation, TargetType player, byte paletteIndex, PaletteColor paletteColor);

	void RemoveCube(InstantiatedCube cubeInstance);

	void RemoveAllCubesGarage();

	void RemoveAllCubes();

	void RemoveObsoleteCubes(MachineModel model);

	void RemoveAllCubesByItemDescriptor(int itemDescriptorKey);
}
