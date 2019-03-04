using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory("Tutorial")]
	[Tooltip("Force mothership to switch into test mode")]
	public class HideTutorialRobotNode : PlayMakerCustomNodeBase
	{
		public override void OnBegin()
		{
			HideTutorialRobotNodeInputParameters inputParameter = new HideTutorialRobotNodeInputParameters();
			base.CommandsHandler.InvokeCommand(OnCommandExecutionFinished, inputParameter);
		}

		public void OnCommandExecutionFinished()
		{
			this.Finish();
		}
	}
}
