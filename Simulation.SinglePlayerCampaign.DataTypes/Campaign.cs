using SinglePlayerCampaign.GUI.Mothership;

namespace Simulation.SinglePlayerCampaign.DataTypes
{
	internal struct Campaign
	{
		public readonly string CampaignID;

		public readonly CampaignType campaignType;

		public readonly string[] ExcludeTheseCubeIDs;

		public readonly ItemCategory[] ExcludeTheseCubeTypes;

		public readonly int MinCPU;

		public readonly int MaxCPU;

		public readonly string CampaignName;

		public readonly string CampaignDesc;

		public readonly string CampaignImage;

		public readonly string[] Rules;

		public readonly string[][] Parameters;

		public readonly CampaignDifficultySetting[] DifficultySettings;

		public readonly int[] PlayerCompletedWaves;

		public readonly bool[] difficultiesCompletedPerCampaign;

		public readonly string MapName;

		public readonly WaveData[] campaignWaves;

		public Campaign(string campaignID, CampaignType campaignType, string[] excludeCubeIds, ItemCategory[] excludeCubeTypes, int minCpu, int maxCpu, string campaignName, string campaignDesc, string campaignImage, string[] rules, string[][] parameters, CampaignDifficultySetting[] difficultySettings, int[] playerCompletedWaves, bool[] difficultiesCompletedPerCampaign, string mapName, WaveData[] campaignWaves)
		{
			CampaignID = campaignID;
			this.campaignType = campaignType;
			ExcludeTheseCubeIDs = excludeCubeIds;
			ExcludeTheseCubeTypes = excludeCubeTypes;
			MinCPU = minCpu;
			MaxCPU = maxCpu;
			CampaignName = campaignName;
			CampaignDesc = campaignDesc;
			CampaignImage = campaignImage;
			Rules = rules;
			Parameters = parameters;
			DifficultySettings = difficultySettings;
			PlayerCompletedWaves = playerCompletedWaves;
			this.difficultiesCompletedPerCampaign = difficultiesCompletedPerCampaign;
			MapName = mapName;
			this.campaignWaves = campaignWaves;
		}
	}
}
