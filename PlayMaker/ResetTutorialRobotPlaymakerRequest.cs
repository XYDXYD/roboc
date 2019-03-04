using System;

namespace PlayMaker
{
	public class ResetTutorialRobotPlaymakerRequest : PlayMakerDataRequestBase<ResetTutorialRobotRequestInputParameters, ResetTutorialRobotPlaymakerRequestReturn>
	{
		public ResetTutorialRobotPlaymakerRequest(ResetTutorialRobotRequestInputParameters inputParameters, Action<ResetTutorialRobotPlaymakerRequestReturn> resultCallback)
			: base(inputParameters, resultCallback)
		{
		}
	}
}
