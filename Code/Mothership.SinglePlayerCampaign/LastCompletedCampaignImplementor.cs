namespace Mothership.SinglePlayerCampaign
{
	internal class LastCompletedCampaignImplementor : ILastCompletedCampaignComponent
	{
		public string campaignId
		{
			get;
			private set;
		}

		public int difficulty
		{
			get;
			private set;
		}

		public LastCompletedCampaignImplementor(string campaignName_, int difficulty_)
		{
			campaignId = campaignName_;
			difficulty = difficulty_;
		}
	}
}
