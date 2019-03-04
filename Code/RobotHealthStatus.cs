using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using System.Collections.Generic;

internal class RobotHealthStatus
{
	internal sealed class RobotStatus
	{
		public Dictionary<CubeTypeID, int> numCubesInRobot = new Dictionary<CubeTypeID, int>();
	}

	public void AddNewRobot(int machineId, TargetType type, FasterList<InstantiatedCube> allCubes, RobotHealthStatusContainer robotHealthStatusContainer, PlayerMachinesContainer playerMachinesContainer)
	{
		RobotStatus robotStatus = new RobotStatus();
		for (int i = 0; i < allCubes.get_Count(); i++)
		{
			InstantiatedCube instantiatedCube = allCubes.get_Item(i);
			if (IsImportantCube(instantiatedCube.persistentCubeData.category))
			{
				CubeTypeID cubeType = instantiatedCube.persistentCubeData.cubeType;
				if (!robotStatus.numCubesInRobot.ContainsKey(cubeType))
				{
					robotStatus.numCubesInRobot[cubeType] = 0;
				}
				Dictionary<CubeTypeID, int> numCubesInRobot;
				CubeTypeID key;
				(numCubesInRobot = robotStatus.numCubesInRobot)[key = cubeType] = numCubesInRobot[key] + 1;
			}
		}
		robotHealthStatusContainer.SetRoboStatus(type, machineId, robotStatus);
	}

	public void RemoveDestroyedCubes(FasterList<InstantiatedCube> destroyedCubes, RobotStatus status)
	{
		for (int i = 0; i < destroyedCubes.get_Count(); i++)
		{
			InstantiatedCube instantiatedCube = destroyedCubes.get_Item(i);
			if (IsImportantCube(instantiatedCube.persistentCubeData.category))
			{
				Dictionary<CubeTypeID, int> numCubesInRobot;
				CubeTypeID cubeType;
				(numCubesInRobot = status.numCubesInRobot)[cubeType = instantiatedCube.persistentCubeData.cubeType] = numCubesInRobot[cubeType] - 1;
			}
		}
	}

	public void AddRespawnedCubes(FasterList<InstantiatedCube> respawnedCubes, RobotStatus status)
	{
		for (int i = 0; i < respawnedCubes.get_Count(); i++)
		{
			InstantiatedCube instantiatedCube = respawnedCubes.get_Item(i);
			if (IsImportantCube(instantiatedCube.persistentCubeData.category))
			{
				Dictionary<CubeTypeID, int> numCubesInRobot;
				CubeTypeID cubeType;
				(numCubesInRobot = status.numCubesInRobot)[cubeType = instantiatedCube.persistentCubeData.cubeType] = numCubesInRobot[cubeType] + 1;
			}
		}
	}

	public bool IsPlayerDead(RobotStatus status)
	{
		int num = 0;
		foreach (KeyValuePair<CubeTypeID, int> item in status.numCubesInRobot)
		{
			num += item.Value;
			if (num > 1)
			{
				return false;
			}
		}
		return true;
	}

	private bool IsImportantCube(CubeCategory category)
	{
		return category != CubeCategory.Chassis && category != CubeCategory.Cosmetic;
	}
}
