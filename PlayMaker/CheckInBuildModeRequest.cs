using System;

namespace PlayMaker
{
	public class CheckInBuildModeRequest : PlayMakerDataRequestBase<PlaymakerRequestEmptyInputParameters, CheckInBuildModeRequestReturn>
	{
		public CheckInBuildModeRequest(Action<CheckInBuildModeRequestReturn> resultCallback)
			: base((PlaymakerRequestEmptyInputParameters)null, resultCallback)
		{
		}
	}
}
