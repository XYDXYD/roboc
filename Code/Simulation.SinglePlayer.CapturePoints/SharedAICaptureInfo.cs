using BehaviorDesigner.Runtime;
using System;

namespace Simulation.SinglePlayer.CapturePoints
{
	[Serializable]
	public class SharedAICaptureInfo : SharedVariable<AICaptureInfo>
	{
		public static implicit operator SharedAICaptureInfo(AICaptureInfo value)
		{
			SharedAICaptureInfo sharedAICaptureInfo = new SharedAICaptureInfo();
			sharedAICaptureInfo.set_Value(value);
			return sharedAICaptureInfo;
		}
	}
}
