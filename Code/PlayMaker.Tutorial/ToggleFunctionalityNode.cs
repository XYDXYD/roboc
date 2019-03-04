using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory("Tutorial")]
	[Tooltip("Toggle enabled / disabled state of different features")]
	public class ToggleFunctionalityNode : PlayMakerCustomNodeBase
	{
		[Tooltip("which functionality to turn on or off")]
		[UIHint(/*Could not decode attribute arguments.*/)]
		public FsmEnum elementToToggle;

		public FsmBool stateToToggleTo;

		public override void OnBegin()
		{
			ToggleFunctionalityNodeInputParameters inputParameter = new ToggleFunctionalityNodeInputParameters(elementToToggle, stateToToggleTo.get_Value());
			base.CommandsHandler.InvokeCommand(OnCommandExecutionFinished, inputParameter);
		}

		public void OnCommandExecutionFinished()
		{
			this.Finish();
		}
	}
}
