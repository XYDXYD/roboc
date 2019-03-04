using Svelto.ECS;

namespace Mothership.SinglePlayerCampaign
{
	internal class CampaignDataEntityView : EntityView
	{
		public ICampaignDataComponent campaignDataComponent;

		public CampaignDataEntityView()
			: this()
		{
		}
	}
}
