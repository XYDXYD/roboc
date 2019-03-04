using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory("Tutorial")]
	[Tooltip("Force mothership to switch into test mode")]
	public class LaunchIntoTestModeNode : PlayMakerCustomNodeBase
	{
		public override void OnBegin()
		{
			LaunchIntoTestModeNodeInputParameters inputParameter = new LaunchIntoTestModeNodeInputParameters();
			base.CommandsHandler.InvokeCommand(OnCommandExecutionFinished, inputParameter);
		}

		public void OnCommandExecutionFinished()
		{
			this.Finish();
		}
	}
}
