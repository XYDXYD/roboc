using UnityEngine;

namespace Game.RoboPass.GUI.Components
{
	internal interface IBattleSummaryNextGradeRewardsComponent
	{
		UIGrid NextRewardsGrid
		{
			get;
		}

		GameObject FreeRewardItem
		{
			get;
		}

		GameObject DeluxeRewardItem
		{
			get;
		}

		bool NextGradeRewardsLabelActive
		{
			set;
		}
	}
}
