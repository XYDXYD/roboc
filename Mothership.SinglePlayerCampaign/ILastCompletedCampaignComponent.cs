namespace Mothership.SinglePlayerCampaign
{
	internal interface ILastCompletedCampaignComponent
	{
		string campaignId
		{
			get;
		}

		int difficulty
		{
			get;
		}
	}
}
