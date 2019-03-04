namespace Services.Web.Photon
{
	internal struct LastCompletedCampaign
	{
		public readonly string campaignId;

		public readonly int difficulty;

		public LastCompletedCampaign(string campaignId_, int difficulty_)
		{
			campaignId = campaignId_;
			difficulty = difficulty_;
		}
	}
}
