using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory("Tutorial")]
	[Tooltip("reloads the tutorial robot after a reset.")]
	public class ReloadTutorialRobotNode : PlayMakerCustomNodeBase
	{
		[Tooltip("optional")]
		public FsmEvent finished_event;

		public override void OnBegin()
		{
			ReloadTutorialRobotPlaymakerRequest dataRequest = new ReloadTutorialRobotPlaymakerRequest(new PlaymakerRequestEmptyInputParameters(), OnRequestResultReturned);
			base.RequestsHandler.RequestData(dataRequest);
		}

		public void OnRequestResultReturned(ReloadTutorialRobotPlaymakerRequestReturn requestResult)
		{
			if (finished_event != null)
			{
				this.get_Fsm().Event(finished_event);
			}
			this.Finish();
		}
	}
}
