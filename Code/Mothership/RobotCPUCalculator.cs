using System;
using System.Collections.Generic;
using Utility;

namespace Mothership
{
	internal static class RobotCPUCalculator
	{
		internal static uint CalculateRobotCPU(Dictionary<uint, uint> cubeCounts, ICubeList cubeList, uint maxCosmeticCpuPool)
		{
			uint num = 0u;
			uint num2 = 0u;
			foreach (KeyValuePair<uint, uint> cubeCount in cubeCounts)
			{
				CubeTypeID index = new CubeTypeID(cubeCount.Key);
				uint value = cubeCount.Value;
				CubeTypeData cubeTypeData = cubeList.CubeTypeDataOf(index);
				if (cubeTypeData == null)
				{
					Console.LogWarning("Cube: " + index.ID + " not found. ");
				}
				else
				{
					PersistentCubeData cubeData = cubeTypeData.cubeData;
					if (cubeData.itemType == ItemType.Cosmetic)
					{
						num2 += cubeData.cpuRating * value;
					}
					num += cubeData.cpuRating * value;
				}
			}
			return CalculateRobotActualCPU(num, num2, maxCosmeticCpuPool);
		}

		internal static uint CalculateRobotActualCPU(uint totalCPU, uint totalCosmeticCPU, uint maxCosmeticCpuPool)
		{
			uint num = (totalCosmeticCPU <= maxCosmeticCpuPool) ? totalCosmeticCPU : maxCosmeticCpuPool;
			if (totalCPU >= totalCosmeticCPU)
			{
				return totalCPU - num;
			}
			throw new OverflowException("Cosmetic pool (" + totalCosmeticCPU + ") can't be more than the total CPU (" + num + ")!");
		}
	}
}
