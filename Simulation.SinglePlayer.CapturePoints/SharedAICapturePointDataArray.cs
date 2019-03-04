using BehaviorDesigner.Runtime;
using System;

namespace Simulation.SinglePlayer.CapturePoints
{
	[Serializable]
	public class SharedAICapturePointDataArray : SharedVariable<AICapturePointData[]>
	{
		public static implicit operator SharedAICapturePointDataArray(AICapturePointData[] value)
		{
			SharedAICapturePointDataArray sharedAICapturePointDataArray = new SharedAICapturePointDataArray();
			sharedAICapturePointDataArray.set_Value(value);
			return sharedAICapturePointDataArray;
		}
	}
}
