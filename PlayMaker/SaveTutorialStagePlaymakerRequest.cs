using System;

namespace PlayMaker
{
	public class SaveTutorialStagePlaymakerRequest : PlayMakerDataRequestBase<SaveTutorialStagePlaymakerRequestInputParameters, SaveTutorialStagePlaymakerRequestReturn>
	{
		public SaveTutorialStagePlaymakerRequest(SaveTutorialStagePlaymakerRequestInputParameters inputParameters, Action<SaveTutorialStagePlaymakerRequestReturn> resultCallback)
			: base(inputParameters, resultCallback)
		{
		}
	}
}
