using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory("Tutorial")]
	[Tooltip("Trigger when the final state is reached")]
	public class FinishedReachedTutorialNode : PlayMakerCustomNodeBase
	{
		public override void OnBegin()
		{
			FinishedReachedTutorialNodeInputParameters inputParameter = new FinishedReachedTutorialNodeInputParameters();
			base.CommandsHandler.InvokeCommand(OnCommandExecutionFinished, inputParameter);
		}

		public void OnCommandExecutionFinished()
		{
			this.Finish();
		}
	}
}
