using UnityEngine;

public static class NetworkIpInfo
{
	public static bool IsMe(string ip)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		NetworkPlayer player = Network.get_player();
		string ipAddress = player.get_ipAddress();
		return string.Compare(ipAddress, ip) == 0;
	}

	public static string GetMyIp()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		NetworkPlayer player = Network.get_player();
		return player.get_ipAddress();
	}
}
