using System;

namespace PlayMaker.Tutorial
{
	public class CheckActiveScreensRequest : PlayMakerDataRequestBase<PlaymakerRequestEmptyInputParameters, CheckActiveScreensRequestReturn>
	{
		public CheckActiveScreensRequest(Action<CheckActiveScreensRequestReturn> resultCallback)
			: base((PlaymakerRequestEmptyInputParameters)null, resultCallback)
		{
		}
	}
}
