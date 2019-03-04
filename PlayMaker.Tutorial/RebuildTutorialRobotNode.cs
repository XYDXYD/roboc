using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory("Tutorial")]
	[Tooltip("Force mothership to switch into test mode")]
	public class RebuildTutorialRobotNode : PlayMakerCustomNodeBase
	{
		public override void OnBegin()
		{
			RebuildTutorialRobotNodeInputParameters inputParameter = new RebuildTutorialRobotNodeInputParameters();
			base.CommandsHandler.InvokeCommand(OnCommandExecutionFinished, inputParameter);
		}

		public void OnCommandExecutionFinished()
		{
			this.Finish();
		}
	}
}
