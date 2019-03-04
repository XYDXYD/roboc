using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services.Analytics
{
	public class LogPlayerKillDependency
	{
		public string gameModeType;

		public bool isRanked;

		public bool isBrawl;

		public bool isCustomGame;

		public string gameGUID;

		public string mapName;

		public int teamId;

		public string killerUsername;

		public bool killerIsAIBot;

		public Vector3 killerPosition;

		public float killerHealth;

		public int killerTeamId;

		public string killerRobotUniqueId = string.Empty;

		public string killerRobotName = string.Empty;

		public int lastDamage;

		public string victimUsername;

		public bool victimIsAIBot;

		public Vector3 victimPosition;

		public float distance;

		public int killerTier;

		public int victimTier;

		public Dictionary<string, object> ToDictionary()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["GameModeType"] = gameModeType;
			dictionary["IsRanked"] = isRanked;
			dictionary["IsBrawl"] = isBrawl;
			dictionary["IsCustomGame"] = isCustomGame;
			dictionary["ReconnectGameGUID"] = gameGUID;
			dictionary["MapName"] = mapName;
			dictionary["KillerUsername"] = killerUsername;
			dictionary["KillerIsAIBot"] = killerIsAIBot;
			dictionary["KillerHealth"] = Convert.ToInt32(killerHealth);
			dictionary["KillerTeam"] = killerTeamId.ToString();
			dictionary["KillerPosX"] = killerPosition.x.ToString();
			dictionary["KillerPosY"] = killerPosition.y.ToString();
			dictionary["KillerPosZ"] = killerPosition.z.ToString();
			dictionary["RobotUniqueID"] = killerRobotUniqueId;
			dictionary["Name"] = killerRobotName;
			dictionary["VictimUsername"] = victimUsername;
			dictionary["VictimIsAIBot"] = victimIsAIBot;
			dictionary["VictimPosX"] = victimPosition.x.ToString();
			dictionary["VictimPosY"] = victimPosition.y.ToString();
			dictionary["VictimPosZ"] = victimPosition.z.ToString();
			dictionary["LastDamage"] = lastDamage;
			dictionary["Distance"] = distance.ToString();
			dictionary["RobotTier"] = killerTier;
			dictionary["VictimRobotTier"] = victimTier;
			return dictionary;
		}
	}
}
