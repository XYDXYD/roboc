using DG.Tweening;

namespace Mothership.GUI
{
	internal interface IRobotRankingWidgetComponent
	{
		uint RobotRankingAbs
		{
			set;
		}

		float RobotRankingRel
		{
			get;
			set;
		}

		float TweenDuration
		{
			get;
		}

		Sequence MainRRSequence
		{
			get;
			set;
		}
	}
}
