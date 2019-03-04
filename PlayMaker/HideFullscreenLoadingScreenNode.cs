using HutongGames.PlayMaker;

namespace PlayMaker
{
	[ActionCategory("Tutorial")]
	public class HideFullscreenLoadingScreenNode : PlayMakerCustomNodeBase
	{
		[Tooltip("finish event")]
		public FsmEvent finishedEvent;

		public override void OnBegin()
		{
			HideFullscreenLoadingScreenNodeCommandParameters inputParameter = new HideFullscreenLoadingScreenNodeCommandParameters();
			base.CommandsHandler.InvokeCommand(OnCommandExecutionFinished, inputParameter);
		}

		public void OnCommandExecutionFinished()
		{
			this.get_Fsm().Event(finishedEvent);
			this.Finish();
		}
	}
}
