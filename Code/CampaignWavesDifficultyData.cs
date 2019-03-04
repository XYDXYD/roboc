using Simulation.SinglePlayerCampaign.DataTypes;
using Svelto.DataStructures;

internal class CampaignWavesDifficultyData
{
	public readonly CampaignDifficultySetting CampaignDifficulty;

	public readonly FasterList<WaveData> WavesData;

	public CampaignWavesDifficultyData(CampaignDifficultySetting campaignDifficulty, FasterList<WaveData> wavesData)
	{
		CampaignDifficulty = campaignDifficulty;
		WavesData = wavesData;
	}
}
