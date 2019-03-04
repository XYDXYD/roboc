using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory("Tutorial")]
	[Tooltip("load enemy robot into test mode")]
	public class SpawnEnemyIntoTestModeNode : PlayMakerCustomNodeBase
	{
		public override void OnBegin()
		{
			SpawnEnemyIntoTestModeNodeParameters inputParameter = new SpawnEnemyIntoTestModeNodeParameters();
			base.CommandsHandler.InvokeCommand(OnCommandExecutionFinished, inputParameter);
		}

		public void OnCommandExecutionFinished()
		{
			this.Finish();
		}
	}
}
