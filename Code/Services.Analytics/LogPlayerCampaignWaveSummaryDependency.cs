using System.Collections.Generic;

namespace Services.Analytics
{
	internal class LogPlayerCampaignWaveSummaryDependency
	{
		public readonly string campaignName;

		public readonly int campaignDifficulty;

		public readonly string campaignDifficultyText;

		public readonly string mapName;

		public readonly string campaignId;

		public readonly string campaignNameKey;

		public readonly int currentWave;

		public readonly string currentWaveStr;

		public readonly int waveDuration;

		public readonly string waveResult;

		public readonly int killedEnemies;

		public readonly int death;

		public readonly int remainingLives;

		public readonly string robotName;

		public readonly string robotUniqueID;

		public readonly int robotCPU;

		public readonly string gameEndReason;

		public LogPlayerCampaignWaveSummaryDependency(string campaignName_, int campaignDifficulty_, string campaignDifficultyText_, string mapName_, string campaignId_, string campaignNameKey_, int currentWave_, int waveDuration_, string waveResult_, int killedEnemies_, int death_, int remainingLives_, string robotName_, string robotUniqueID_, int robotCPU_, string gameEndReason_)
		{
			campaignName = campaignName_;
			campaignDifficulty = campaignDifficulty_;
			campaignDifficultyText = campaignDifficultyText_;
			mapName = mapName_;
			campaignId = campaignId_;
			campaignNameKey = campaignNameKey_;
			currentWaveStr = "wave " + currentWave_;
			currentWave = currentWave_;
			waveDuration = waveDuration_;
			waveResult = waveResult_;
			killedEnemies = killedEnemies_;
			death = death_;
			remainingLives = remainingLives_;
			robotName = robotName_;
			robotUniqueID = robotUniqueID_;
			robotCPU = robotCPU_;
			gameEndReason = gameEndReason_;
		}

		public Dictionary<string, object> ToDictionary()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["CampaignName"] = campaignName;
			dictionary["CampaignDifficulty"] = campaignDifficulty;
			dictionary["CampaignDifficultyText"] = campaignDifficultyText;
			dictionary["MapName"] = mapName;
			dictionary["CampaignId"] = campaignId;
			dictionary["CurrentWave"] = currentWaveStr;
			dictionary["WaveDuration"] = waveDuration;
			dictionary["WaveResult"] = waveResult;
			dictionary["KilledEnemies"] = killedEnemies;
			dictionary["Death"] = death;
			dictionary["RemainingLives"] = remainingLives;
			dictionary["Name"] = robotName;
			dictionary["RobotUniqueID"] = robotUniqueID;
			dictionary["RobotCPU"] = robotCPU;
			dictionary["GameEndReason"] = gameEndReason;
			return dictionary;
		}
	}
}
