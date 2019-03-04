using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory("Tutorial")]
	[Tooltip("locks the switching of  tool mode")]
	public class SwitchingLockToolModeNode : PlayMakerCustomNodeBase
	{
		public FsmBool lockSwitching;

		public override void OnBegin()
		{
			SwitchingLockToolModeInputParameters inputParameter = new SwitchingLockToolModeInputParameters(lockSwitching.get_Value());
			base.CommandsHandler.InvokeCommand(OnCommandExecutionFinished, inputParameter);
		}

		public void OnCommandExecutionFinished()
		{
			this.Finish();
		}
	}
}
