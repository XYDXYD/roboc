using System.Collections.Generic;

namespace Services.Analytics
{
	internal class LogPlayerStartedGameDependency
	{
		public string gameModeType;

		public bool isRanked;

		public bool isBrawl;

		public bool isCustomGame;

		public string reconnectGameGUID;

		public string mapName;

		public int teamId;

		public PlayerRobotStats machineStats;

		public bool isReconnecting;

		public string sceneName;

		public string gameModeTypeForAnalytics = string.Empty;

		public uint robotCPU;

		public bool isCRFBot;

		public string controlType;

		public bool verticalStrafing;

		public int totalCosmetics;

		public bool sendShortVersion;

		public uint? tier;

		public uint? aiPlayers;

		public LogPlayerStartedGameDependency(int teamId_, PlayerRobotStats machineStats_, bool isReconnecting_)
		{
			teamId = teamId_;
			machineStats = machineStats_;
			isReconnecting = isReconnecting_;
		}

		public Dictionary<string, object> ToDictionary()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["GameModeType"] = gameModeType;
			dictionary["IsRanked"] = isRanked;
			dictionary["IsBrawl"] = isBrawl;
			dictionary["IsCustomGame"] = isCustomGame;
			dictionary["ReconnectGameGUID"] = reconnectGameGUID;
			dictionary["MapName"] = mapName;
			dictionary["TeamId"] = teamId;
			dictionary["RobotCPU"] = machineStats.totalCPU;
			dictionary["MMR"] = 0;
			dictionary["TeamMMR"] = 0;
			dictionary["DamageBoost"] = machineStats.damageBoost;
			dictionary["BaseHealth"] = machineStats.baseHealth;
			dictionary["HealthBoost"] = machineStats.healthBoost;
			dictionary["BaseSpeed"] = machineStats.baseSpeed;
			dictionary["SpeedBoost"] = machineStats.speedBoost;
			dictionary["CosmeticCPU"] = machineStats.cosmeticCPU;
			dictionary["MasteryLevel"] = machineStats.masteryLevel;
			dictionary["Mass"] = machineStats.mass;
			dictionary["IsReconnecting"] = isReconnecting;
			dictionary["RobotUniqueID"] = machineStats.robotUniqueId;
			dictionary["Name"] = machineStats.robotName;
			dictionary["RobotRanking"] = machineStats.ranking;
			dictionary["RobotTier"] = (int)(tier.HasValue ? tier.Value : 0);
			return dictionary;
		}
	}
}
