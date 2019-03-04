using UnityEngine;

namespace Simulation.BattleArena.CapturePoint
{
	internal interface IAnimationComponent
	{
		Animator animator
		{
			get;
		}

		string captureCompletedTrigger
		{
			get;
		}

		string segmentCapturedTrigger
		{
			get;
		}
	}
}
