namespace PlayMaker
{
	public class HideTutorialDialog : PlayMakerCustomNodeBase
	{
		public override void OnBegin()
		{
			HideTutorialDialogCommandParameters inputParameter = new HideTutorialDialogCommandParameters();
			base.CommandsHandler.InvokeCommand(OnCommandExecutionFinished, inputParameter);
		}

		public void OnCommandExecutionFinished()
		{
			this.Finish();
		}
	}
}
