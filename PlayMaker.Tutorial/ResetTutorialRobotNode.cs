using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory("Tutorial")]
	[Tooltip("reset tutorial robot node")]
	public class ResetTutorialRobotNode : PlayMakerCustomNodeBase
	{
		[Tooltip("optional")]
		public FsmEvent finished_event;

		[Tooltip("0 is the first robot.")]
		public FsmInt stageToResetTo;

		public override void OnBegin()
		{
			ResetTutorialRobotPlaymakerRequest dataRequest = new ResetTutorialRobotPlaymakerRequest(new ResetTutorialRobotRequestInputParameters(stageToResetTo.get_Value()), OnRequestResultReturned);
			base.RequestsHandler.RequestData(dataRequest);
		}

		public void OnRequestResultReturned(ResetTutorialRobotPlaymakerRequestReturn requestResult)
		{
			if (finished_event != null)
			{
				this.get_Fsm().Event(finished_event);
			}
			this.Finish();
		}
	}
}
