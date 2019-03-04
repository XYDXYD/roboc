using Simulation;
using System;
using System.Collections.Generic;

namespace Services.Analytics
{
	internal class LogPlayerEndedGameDependency
	{
		public Dictionary<string, uint> playerBattleScore;

		public string gameModeType;

		public bool isRanked;

		public bool isBrawl;

		public bool isCustomGame;

		public string reconnectGameGUID;

		public string mapName;

		public int teamId;

		public int gameDurationSecs;

		public string gameEndResult;

		public string gameEndReason;

		public uint scoreTeamScore;

		public int totalDamageReceived;

		public string robotUniqueID = string.Empty;

		public string robotName = string.Empty;

		public string sceneName;

		public string gameModeTypeForAnalytics = string.Empty;

		public GameStateResult battleEndResult;

		public string gameServerErrorCode;

		public int robotCpu;

		public int robotRanking;

		public uint? tier;

		public int duration;

		public LogPlayerEndedGameDependency(int teamId_, int gameDurationSecs_, string gameResult_, string endReason_, int robotCpu_, int robotRanking_, int duration_)
		{
			teamId = teamId_;
			gameDurationSecs = gameDurationSecs_;
			gameEndResult = gameResult_;
			gameEndReason = endReason_;
			robotCpu = robotCpu_;
			robotRanking = robotRanking_;
			duration = duration_;
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
			dictionary["GameDurationSecs"] = gameDurationSecs;
			dictionary["GameEndResult"] = gameEndResult;
			dictionary["GameEndReason"] = gameEndReason;
			dictionary["ScoreKills"] = Convert.ToInt32(playerBattleScore["scoreKills"]);
			dictionary["ScoreDeaths"] = Convert.ToInt32(playerBattleScore["scoreRobotDestroyed"]);
			dictionary["ScoreAssists"] = Convert.ToInt32(playerBattleScore["scoreKillAssist"]);
			dictionary["ScoreHeal"] = Convert.ToInt32(playerBattleScore["scoreHealCubes"]);
			dictionary["ScoreDamage"] = Convert.ToInt32(playerBattleScore["scoreDestroyedCubes"]);
			dictionary["ScorePlayerScore"] = Convert.ToInt32(playerBattleScore["userScore"]);
			dictionary["ScoreTeamScore"] = Convert.ToInt32(scoreTeamScore);
			dictionary["TotalDamageReceived"] = totalDamageReceived;
			dictionary["RobotUniqueID"] = robotUniqueID;
			dictionary["Name"] = robotName;
			dictionary["RobotCPU"] = robotCpu;
			dictionary["RobotRanking"] = robotRanking;
			dictionary["RobotTier"] = (int)(tier.HasValue ? tier.Value : 0);
			return dictionary;
		}
	}
}
