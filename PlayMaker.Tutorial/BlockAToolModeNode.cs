using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory("Tutorial")]
	[Tooltip("prevents switching to a specific tool and also hides that tool from the icons of available tools")]
	public class BlockAToolModeNode : PlayMakerCustomNodeBase
	{
		public FsmEnum toolToBlock;

		public FsmBool blockOrUnblock;

		public override void OnBegin()
		{
			BlockAToolModeInputParameters inputParameter = new BlockAToolModeInputParameters(FsmEnum.op_Implicit(toolToBlock.get_Value()), blockOrUnblock.get_Value());
			base.CommandsHandler.InvokeCommand(OnCommandExecutionFinished, inputParameter);
		}

		public void OnCommandExecutionFinished()
		{
			this.Finish();
		}
	}
}
