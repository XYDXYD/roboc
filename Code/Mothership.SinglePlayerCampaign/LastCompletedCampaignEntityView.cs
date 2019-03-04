using Svelto.ECS;

namespace Mothership.SinglePlayerCampaign
{
	internal class LastCompletedCampaignEntityView : EntityView
	{
		public ILastCompletedCampaignComponent lastCompletedCampaignComponent;

		public LastCompletedCampaignEntityView()
			: this()
		{
		}
	}
}
