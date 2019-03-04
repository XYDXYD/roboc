using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory("Tutorial")]
	[Tooltip("Force mothership to switch into test mode")]
	public class SwitchIntoBuildModeNode : PlayMakerCustomNodeBase
	{
		public override void OnBegin()
		{
			SwitchIntoBuildModeNodeInputParameters inputParameter = new SwitchIntoBuildModeNodeInputParameters();
			base.CommandsHandler.InvokeCommand(OnCommandExecutionFinished, inputParameter);
		}

		public void OnCommandExecutionFinished()
		{
			this.Finish();
		}
	}
}
