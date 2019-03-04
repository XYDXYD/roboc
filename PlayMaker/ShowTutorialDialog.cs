using HutongGames.PlayMaker;

namespace PlayMaker
{
	public class ShowTutorialDialog : PlayMakerCustomNodeBase
	{
		[UIHint(/*Could not decode attribute arguments.*/)]
		[Tooltip("localisation key of body text to display")]
		public FsmString bodyTextToDisplay;

		[Tooltip("time to display the tutorial dialog for (seconds)")]
		public FsmInt timeToDisplaySeconds;

		public override void Reset()
		{
			timeToDisplaySeconds = FsmInt.op_Implicit(0);
		}

		public override void OnBegin()
		{
			ShowTutorialDialogCommandParameters inputParameter = new ShowTutorialDialogCommandParameters(bodyTextToDisplay, timeToDisplaySeconds);
			base.CommandsHandler.InvokeCommand(OnCommandExecutionFinished, inputParameter);
		}

		public void OnCommandExecutionFinished()
		{
			this.Finish();
		}
	}
}
