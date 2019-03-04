using Svelto.ECS;

namespace Game.RoboPass.GUI.Components
{
	internal interface IBattleSummaryRewardedItemsPanelComponent
	{
		UIGrid RewardsGrid
		{
			get;
		}

		bool PanelActive
		{
			set;
		}

		DispatchOnSet<bool> CollectRewardsClicked
		{
			get;
		}

		DispatchOnSet<bool> ContinueClicked
		{
			get;
		}

		DispatchOnSet<bool> GetRoboPassPlusClicked
		{
			get;
		}

		string TitleMsg
		{
			set;
		}

		string DescMsg
		{
			set;
		}

		string TitleRewardUnlocked
		{
			get;
		}

		string TitleRewardReceived
		{
			get;
		}

		string DescRewardUnlocked
		{
			get;
		}

		string DescRewardReceived
		{
			get;
		}

		bool ButtonCollectActive
		{
			set;
		}

		bool ButtonContinueActive
		{
			set;
		}

		bool ButtonGetRoboPassPlusActive
		{
			set;
		}

		bool RoboPassPlusRewardTextActive
		{
			set;
		}
	}
}
