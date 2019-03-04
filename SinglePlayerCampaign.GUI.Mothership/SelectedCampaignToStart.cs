namespace SinglePlayerCampaign.GUI.Mothership
{
	public class SelectedCampaignToStart
	{
		public readonly string CampaignID;

		public readonly string CampaignMap;

		public readonly int Difficulty;

		public readonly string CampaignName;

		public SelectedCampaignToStart(string campaignId, int difficulty, string campaignName, string campaignMap)
		{
			CampaignID = campaignId;
			CampaignMap = campaignMap;
			Difficulty = difficulty;
			CampaignName = campaignName;
		}
	}
}
