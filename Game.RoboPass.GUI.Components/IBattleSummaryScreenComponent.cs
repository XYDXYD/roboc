using Svelto.ECS;
using UnityEngine;

namespace Game.RoboPass.GUI.Components
{
	public interface IBattleSummaryScreenComponent
	{
		DispatchOnSet<bool> ContinueClicked
		{
			get;
		}

		DispatchOnChange<bool> OnScreenDisplayChange
		{
			get;
		}

		bool ScreenActive
		{
			set;
		}

		bool ContinueButtonActive
		{
			set;
		}

		string CurrentSeasonLabel
		{
			set;
		}

		string CurrentGradeLabel
		{
			set;
		}

		Animation Animation
		{
			get;
		}

		string LevelUpAnimationPart1
		{
			get;
		}

		string LevelUpAnimationPart2
		{
			get;
		}
	}
}
