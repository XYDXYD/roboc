namespace SinglePlayerServiceLayer.Requests.Photon
{
	public class GetCampaignWavesDependency
	{
		public readonly string CampaignID;

		public readonly int CampaignDifficulty;

		public GetCampaignWavesDependency(string campaignId, int campaignDifficulty)
		{
			CampaignID = campaignId;
			CampaignDifficulty = campaignDifficulty;
		}
	}
}
