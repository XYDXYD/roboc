using System;

namespace PlayMaker
{
	public class SaveTutorialBayPlaymakerRequest : PlayMakerDataRequestBase<PlaymakerRequestEmptyInputParameters, SaveTutorialBayPlaymakerRequestReturn>
	{
		public SaveTutorialBayPlaymakerRequest(Action<SaveTutorialBayPlaymakerRequestReturn> resultCallback)
			: base(new PlaymakerRequestEmptyInputParameters(), resultCallback)
		{
		}
	}
}
