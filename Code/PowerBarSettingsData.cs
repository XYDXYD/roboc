using ExitGames.Client.Photon;
using System;

internal class PowerBarSettingsData
{
	public readonly float RefillRatePerSecond;

	public readonly uint PowerForAllRobots;

	public PowerBarSettingsData(Hashtable value)
	{
		RefillRatePerSecond = Convert.ToSingle(value.get_Item((object)"refillRatePerSecond"));
		PowerForAllRobots = Convert.ToUInt32(value.get_Item((object)"powerForAllRobots"));
	}
}
