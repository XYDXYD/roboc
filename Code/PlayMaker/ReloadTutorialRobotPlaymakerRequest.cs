using System;

namespace PlayMaker
{
	public class ReloadTutorialRobotPlaymakerRequest : PlayMakerDataRequestBase<PlaymakerRequestEmptyInputParameters, ReloadTutorialRobotPlaymakerRequestReturn>
	{
		public ReloadTutorialRobotPlaymakerRequest(PlaymakerRequestEmptyInputParameters inputParameters, Action<ReloadTutorialRobotPlaymakerRequestReturn> resultCallback)
			: base(inputParameters, resultCallback)
		{
		}
	}
}
