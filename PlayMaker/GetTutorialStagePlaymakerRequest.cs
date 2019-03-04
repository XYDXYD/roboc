using System;

namespace PlayMaker
{
	public class GetTutorialStagePlaymakerRequest : PlayMakerDataRequestBase<PlaymakerRequestEmptyInputParameters, GetTutorialStagePlaymakerRequestResult>
	{
		public GetTutorialStagePlaymakerRequest(Action<GetTutorialStagePlaymakerRequestResult> resultCallback)
			: base((PlaymakerRequestEmptyInputParameters)null, resultCallback)
		{
		}
	}
}
