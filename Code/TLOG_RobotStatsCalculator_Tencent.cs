using Mothership;
using Simulation;
using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

internal class TLOG_RobotStatsCalculator_Tencent
{
	public const int HEALTH_BOOST_MULTIPLIER = 10000000;

	private bool _useDecimalSystem;

	private MovementStats _movementStats;

	private DamageBoostDeserialisedData _damageBoostData;

	private ICubeList cubeList;

	private IEditorCubeFactory cubeFactory;

	public TLOG_RobotStatsCalculator_Tencent(MovementStats movementStats, DamageBoostDeserialisedData damageBoostData, bool useDecimalSystem)
	{
		_useDecimalSystem = useDecimalSystem;
		_movementStats = movementStats;
		_damageBoostData = damageBoostData;
	}

	public void SetCubeData(ICubeList cubeList_, IEditorCubeFactory cubeFactory_)
	{
		cubeList = cubeList_;
		cubeFactory = cubeFactory_;
	}

	public PlayerRobotStats CalculateRobotStats(byte[] robotData, string robotUniqueId, int masteryLevel)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		Mothership.IMachineMap machineMap = new Mothership.MachineMap();
		MachineModel machineModel = new MachineModel(robotData);
		CubeNodeInstance cubeNodeInstance = new CubeNodeInstance();
		FasterReadOnlyList<CubeData> cubes = machineModel.DTO.cubes;
		for (int i = 0; i < cubes.get_Count(); i++)
		{
			CubeData cubeData = cubes.get_Item(i);
			InstantiatedCube instantiatedCube = new InstantiatedCube(cubeNodeInstance, cubeList.CubeTypeDataOf(cubeData.iID).cubeData, cubeData.gridLocation, cubeData.rotationIndex);
			instantiatedCube.paletteIndex = cubeData.paletteIndex;
			cubeNodeInstance.instantiatedCube = instantiatedCube;
			GameObject val = cubeFactory.BuildCube(cubeData.iID, GridScaleUtility.GridToWorld(cubeData.gridLocation, TargetType.Player), CubeData.IndexToQuat(cubeData.rotationIndex), TargetType.Player);
			instantiatedCube.SetParams(val.GetComponent<CubeInstance>());
			machineMap.SilentlyAddCellToMachineMap(cubeData.gridLocation, instantiatedCube, val);
		}
		PlayerRobotStats result = CalculateRobotStats(machineMap, robotUniqueId, masteryLevel);
		HashSet<InstantiatedCube> allInstantiatedCubes = machineMap.GetAllInstantiatedCubes();
		HashSet<InstantiatedCube>.Enumerator enumerator = allInstantiatedCubes.GetEnumerator();
		using (HashSet<InstantiatedCube>.Enumerator enumerator2 = enumerator)
		{
			while (enumerator2.MoveNext())
			{
				GameObject cubeAt = machineMap.GetCubeAt(enumerator2.Current.gridPos);
				Object.Destroy(cubeAt);
			}
			return result;
		}
	}

	public PlayerRobotStats CalculateRobotStats(Simulation.IMachineMap machineMap, string robotUniqueId, int masteryLevel)
	{
		MachineCell cell = null;
		return CalculateRobotStatsInternal(ref cell, (IEnumerable<InstantiatedCube>)machineMap.GetAllInstantiatedCubes(), robotUniqueId, masteryLevel, delegate(Byte3 cellLocation)
		{
			cell = machineMap.GetCellAt(cellLocation);
		});
	}

	public PlayerRobotStats CalculateRobotStats(Mothership.IMachineMap machineMap, string robotUniqueId, int masteryLevel)
	{
		MachineCell cell = null;
		return CalculateRobotStatsInternal(ref cell, machineMap.GetAllInstantiatedCubes(), robotUniqueId, masteryLevel, delegate(Byte3 cellLocation)
		{
			cell = machineMap.GetCellAt(cellLocation);
		});
	}

	private PlayerRobotStats CalculateRobotStatsInternal(ref MachineCell cell, IEnumerable<InstantiatedCube> cubes, string robotUniqueId, int masteryLevel, Action<Byte3> GetCellAt)
	{
		PlayerRobotStats playerRobotStats = new PlayerRobotStats();
		playerRobotStats.robotUniqueId = robotUniqueId;
		playerRobotStats.masteryLevel = masteryLevel;
		MachineSpeedUtility machineSpeedUtility = new MachineSpeedUtility(_movementStats, _useDecimalSystem);
		float num = 0f;
		int num2 = 0;
		int num3 = 0;
		Dictionary<ItemCategory, int> dictionary = new Dictionary<ItemCategory, int>();
		FasterList<MachineCell> val = new FasterList<MachineCell>();
		foreach (InstantiatedCube cube in cubes)
		{
			PersistentCubeData persistentCubeData = cube.persistentCubeData;
			ItemType itemType = persistentCubeData.itemType;
			ItemDescriptor itemDescriptor = cube.persistentCubeData.itemDescriptor;
			num2 += MachineMassUtility.GetMass(cube);
			if (itemType == ItemType.Cosmetic)
			{
				playerRobotStats.cosmeticCPU += (int)persistentCubeData.cpuRating;
			}
			else
			{
				playerRobotStats.totalCPU += (int)persistentCubeData.cpuRating;
				playerRobotStats.ranking += persistentCubeData.cubeRanking;
				playerRobotStats.baseHealth += persistentCubeData.health;
				num += (float)(int)(persistentCubeData.healthBoost * 1E+07f);
			}
			if (itemType == ItemType.Movement)
			{
				ItemCategory itemCategory = itemDescriptor.itemCategory;
				if (_movementStats.data.TryGetValue(itemDescriptor.GenerateKey(), out MovementStatsData _))
				{
					GetCellAt(cube.gridPos);
					MachineCell machineCell = cell;
					val.Add(machineCell);
					ItemCategory groupedMovementCategory = GetGroupedMovementCategory(itemCategory);
					if (!dictionary.ContainsKey(groupedMovementCategory))
					{
						dictionary[groupedMovementCategory] = 0;
					}
					Dictionary<ItemCategory, int> dictionary2;
					ItemCategory key;
					(dictionary2 = dictionary)[key = groupedMovementCategory] = dictionary2[key] + 1;
					if (itemCategory != ItemCategory.Thruster && itemCategory != ItemCategory.Propeller)
					{
						num3--;
					}
				}
				else
				{
					Console.LogError("Unable to get movement stats for " + itemCategory + " " + itemDescriptor.itemSize);
				}
			}
		}
		int baseSpeed = 0;
		float baseSpeedPercent = 0f;
		float speedBoostPercent = 0f;
		machineSpeedUtility.CalculateSpeed(val, dictionary, num3, out baseSpeed, out baseSpeedPercent, out speedBoostPercent);
		DamageBoostUtility damageBoostUtility = new DamageBoostUtility((DamageBoostDeserialisedData)_damageBoostData.Clone());
		float num4 = damageBoostUtility.CurrentRobotDamageBoostPercentage((uint)playerRobotStats.totalCPU);
		playerRobotStats.damageBoost = Convert.ToInt32(num4 * 10000f);
		playerRobotStats.mass = Convert.ToInt32(MachineMassUtility.GetDisplayMass(num2));
		playerRobotStats.healthBoost = Convert.ToInt32(num / 1E+07f * 10000f);
		playerRobotStats.baseSpeed = baseSpeed;
		playerRobotStats.speedBoost = Convert.ToInt32(speedBoostPercent * 100000f);
		return playerRobotStats;
	}

	private ItemCategory GetGroupedMovementCategory(ItemCategory itemCategory)
	{
		ItemCategory result = itemCategory;
		if (itemCategory == ItemCategory.SprinterLeg || itemCategory == ItemCategory.MechLeg)
		{
			result = ItemCategory.SprinterLeg;
		}
		return result;
	}
}
