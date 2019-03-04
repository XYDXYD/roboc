using System;

namespace Simulation.SinglePlayer.CapturePoints
{
	[Serializable]
	public class AICaptureInfo
	{
		public AICapturePointData Goal
		{
			get;
			set;
		}

		public bool IsCapturing
		{
			get;
			set;
		}

		public AICaptureInfo()
		{
		}

		public AICaptureInfo(AICapturePointData goal, bool isCapturing)
		{
			Goal = goal;
			IsCapturing = isCapturing;
		}
	}
}
