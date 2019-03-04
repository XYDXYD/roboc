using System;

namespace Battle
{
	internal class BattleParametersData
	{
		public readonly string HostIP;

		public readonly int HostPort;

		public readonly string MapName;

		public readonly GameModeKey GameModeKey;

		public readonly DateTime ReceivedEnterBattleTime;

		public readonly NetworkConfig NetworkConfigs;

		public readonly CustomGameConfigOverrides CustomGameConfig;

		public readonly string GameGuid;

		public BattleParametersData(string hostIP, int hostPort, string mapName, GameModeKey key, DateTime receivedEnterBattleTime, NetworkConfig networkConfig, CustomGameConfigOverrides customConfig, string gameGuid)
		{
			HostIP = hostIP;
			HostPort = hostPort;
			MapName = mapName;
			GameModeKey = key;
			ReceivedEnterBattleTime = receivedEnterBattleTime;
			NetworkConfigs = networkConfig;
			CustomGameConfig = customConfig;
			GameGuid = gameGuid;
		}
	}
}
