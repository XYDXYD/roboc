using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory("Tutorial")]
	[Tooltip("changes current tool mode")]
	public class ChangeToolModeNode : PlayMakerCustomNodeBase
	{
		public FsmEnum inputToolMode;

		public override void OnBegin()
		{
			ChangeToolModeNodeInputParameters inputParameter = new ChangeToolModeNodeInputParameters(inputToolMode.get_Value());
			base.CommandsHandler.InvokeCommand(OnCommandExecutionFinished, inputParameter);
		}

		public void OnCommandExecutionFinished()
		{
			this.Finish();
		}
	}
}
