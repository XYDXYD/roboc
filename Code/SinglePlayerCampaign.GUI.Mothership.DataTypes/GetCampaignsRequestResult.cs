using Simulation.SinglePlayerCampaign.DataTypes;

namespace SinglePlayerCampaign.GUI.Mothership.DataTypes
{
	internal class GetCampaignsRequestResult
	{
		public readonly GameModeVersionParams CampaignVersionParams;

		public readonly Campaign[] CampaignsGameParameters;

		public GetCampaignsRequestResult(GameModeVersionParams campaignVersionParams, Campaign[] campaignsGameParameters)
		{
			CampaignVersionParams = campaignVersionParams;
			CampaignsGameParameters = campaignsGameParameters;
		}
	}
}
