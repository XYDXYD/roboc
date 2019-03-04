using Game.RoboPass.GUI.Components;
using Svelto.ECS;

namespace Game.RoboPass.GUI.EntityViews
{
	internal class RoboPassBattleSummaryScreenEntityView : EntityView
	{
		public IBattleSummaryScreenComponent screenComponent;

		public IBattleSummaryNextGradeRewardsComponent nextGradeRewardsComponent;

		public IBattleSummaryRewardedItemsPanelComponent rewardedItemsPanelComponent;

		public IBattleSummaryRewardItemPurchaseComponent rewardItemPurchaseComponent;

		public IBattleSummaryAnimationComponent battleSummaryAnimationComponent;

		public RoboPassBattleSummaryScreenEntityView()
			: this()
		{
		}
	}
}
