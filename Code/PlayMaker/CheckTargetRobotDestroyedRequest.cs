using System;

namespace PlayMaker
{
	public class CheckTargetRobotDestroyedRequest : PlayMakerDataRequestBase<PlaymakerRequestEmptyInputParameters, CheckTargetRobotDestroyedRequestReturn>
	{
		public CheckTargetRobotDestroyedRequest(Action<CheckTargetRobotDestroyedRequestReturn> resultCallback)
			: base((PlaymakerRequestEmptyInputParameters)null, resultCallback)
		{
		}
	}
}
