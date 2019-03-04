using Simulation.SinglePlayerCampaign.DataTypes;

namespace Mothership.SinglePlayerCampaign
{
	internal class CampaignDataImplementor : ICampaignDataComponent
	{
		public Campaign[] campaignData
		{
			get;
			private set;
		}

		public CampaignDataImplementor(Campaign[] campaignData_)
		{
			campaignData = campaignData_;
		}
	}
}
