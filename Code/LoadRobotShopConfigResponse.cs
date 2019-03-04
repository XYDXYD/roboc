using System.Collections.Generic;

internal class LoadRobotShopConfigResponse
{
	public List<RobotShopCpuRange> robotShopRanges
	{
		get;
		private set;
	}

	public float submissionMultiplier
	{
		get;
		private set;
	}

	public float earningMultiplier
	{
		get;
		private set;
	}

	public LoadRobotShopConfigResponse(int[] robotCpuRanges, float submissionMultiplier, float earningMultiplier)
	{
		List<RobotShopCpuRange> list = new List<RobotShopCpuRange>(robotCpuRanges.Length);
		for (int i = 0; i < robotCpuRanges.Length; i++)
		{
			list.Add(new RobotShopCpuRange(robotCpuRanges[i]));
		}
		robotShopRanges = list;
		this.submissionMultiplier = submissionMultiplier;
		this.earningMultiplier = earningMultiplier;
	}

	public LoadRobotShopConfigResponse(LoadRobotShopConfigResponse source)
	{
		robotShopRanges = new List<RobotShopCpuRange>(source.robotShopRanges);
		submissionMultiplier = source.submissionMultiplier;
		earningMultiplier = source.earningMultiplier;
	}
}
