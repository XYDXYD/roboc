using System.Collections.Generic;
using Utility;

namespace Mothership
{
	internal class RobotShopCpuRangeHelper
	{
		public static RobotShopCpuRange GetRange(List<RobotShopCpuRange> ranges, uint robotRanking)
		{
			for (int i = 0; i < ranges.Count; i++)
			{
				if (robotRanking <= ranges[i].MaxCpu)
				{
					return ranges[i];
				}
			}
			Console.LogError("[RobotShopRange] Incorrect robot ranking " + robotRanking);
			return ranges[ranges.Count - 1];
		}
	}
}
