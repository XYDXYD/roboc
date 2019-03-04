public class UpdatePlayerCompletedCampaignWaveDependency
{
	public readonly string CampaignID;

	public readonly int Difficulty;

	public readonly int WaveNo;

	public UpdatePlayerCompletedCampaignWaveDependency(string campaignID, int difficulty, int waveNo)
	{
		CampaignID = campaignID;
		Difficulty = difficulty;
		WaveNo = waveNo;
	}
}
