using HutongGames.PlayMaker;

namespace PlayMaker
{
	[ActionCategory("Tutorial")]
	public class ShowFullscreenLoadingScreenNode : PlayMakerCustomNodeBase
	{
		[Tooltip("finish event")]
		public FsmEvent finishedEvent;

		public override void OnBegin()
		{
			ShowFullscreenLoadingScreenNodeCommandParameters inputParameter = new ShowFullscreenLoadingScreenNodeCommandParameters();
			base.CommandsHandler.InvokeCommand(OnCommandExecutionFinished, inputParameter);
		}

		public void OnCommandExecutionFinished()
		{
			this.get_Fsm().Event(finishedEvent);
			this.Finish();
		}
	}
}
