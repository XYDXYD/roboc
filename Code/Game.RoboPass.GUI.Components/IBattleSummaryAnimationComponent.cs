using UnityEngine;

namespace Game.RoboPass.GUI.Components
{
	public interface IBattleSummaryAnimationComponent
	{
		float CurrentProgress
		{
			set;
		}

		float NewProgress
		{
			get;
			set;
		}

		float NewProgressTweenDuration
		{
			get;
		}

		GameObject GameObject
		{
			get;
		}
	}
}
