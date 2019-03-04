using HutongGames.PlayMaker;

namespace PlayMaker
{
	public class ToggleShortcutHintsVisibilityNode : PlayMakerCustomNodeBase
	{
		[Tooltip("shortcuts in top left of screen should be visible?")]
		[UIHint(/*Could not decode attribute arguments.*/)]
		public FsmBool ShouldBeVisible;

		public override void OnBegin()
		{
			ToggleShortcutHintsVisibilityCommandParameters inputParameter = new ToggleShortcutHintsVisibilityCommandParameters(FsmBool.op_Implicit(ShouldBeVisible.get_Value()));
			base.CommandsHandler.InvokeCommand(OnCommandExecutionFinished, inputParameter);
		}

		public void OnCommandExecutionFinished()
		{
			this.Finish();
		}
	}
}
