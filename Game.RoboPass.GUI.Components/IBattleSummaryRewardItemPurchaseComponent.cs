using Svelto.ECS;

namespace Game.RoboPass.GUI.Components
{
	public interface IBattleSummaryRewardItemPurchaseComponent
	{
		DispatchOnSet<bool> SummaryPanelBuyNowClicked
		{
			get;
		}

		DispatchOnSet<bool> RewardsPanelBuyNowClicked
		{
			get;
		}
	}
}
