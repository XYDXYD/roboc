using Svelto.ECS;
using UnityEngine;

namespace Game.RoboPass.GUI.Components
{
	internal interface IRewardsGridsComponent
	{
		DispatchOnChange<int> pageChanged
		{
			get;
		}

		GameObject deluxeRewardTemplateGo
		{
			get;
		}

		GameObject freeRewardTemplateGo
		{
			get;
		}

		UIGrid deluxeRewardsUiGrid
		{
			get;
		}

		UIGrid freeRewardsUiGrid
		{
			get;
		}

		bool deluxeRewardsUnlocked
		{
			set;
		}

		int columnLimit
		{
			get;
			set;
		}

		int currentPageNumber
		{
			get;
			set;
		}

		int maxPageNumber
		{
			get;
			set;
		}
	}
}
