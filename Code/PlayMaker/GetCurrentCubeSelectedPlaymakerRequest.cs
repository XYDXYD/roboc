using System;

namespace PlayMaker
{
	public class GetCurrentCubeSelectedPlaymakerRequest : PlayMakerDataRequestBase<PlaymakerRequestEmptyInputParameters, GetCurrentCubeSelectedPlaymakerRequestResult>
	{
		public GetCurrentCubeSelectedPlaymakerRequest(Action<GetCurrentCubeSelectedPlaymakerRequestResult> resultCallback)
			: base((PlaymakerRequestEmptyInputParameters)null, resultCallback)
		{
		}
	}
}
