using System;

namespace PlayMaker
{
	public class CheckInTestModeContextRequest : PlayMakerDataRequestBase<PlaymakerRequestEmptyInputParameters, CheckInTestModeContextRequestReturn>
	{
		public CheckInTestModeContextRequest(Action<CheckInTestModeContextRequestReturn> resultCallback)
			: base((PlaymakerRequestEmptyInputParameters)null, resultCallback)
		{
		}
	}
}
