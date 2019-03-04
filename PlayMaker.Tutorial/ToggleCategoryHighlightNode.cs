using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory("Tutorial")]
	[Tooltip("turns on or off highlighting for a specific category in the cube selector")]
	public class ToggleCategoryHighlightNode : PlayMakerCustomNodeBase
	{
		[Tooltip("highlight on or off")]
		[UIHint(/*Could not decode attribute arguments.*/)]
		public FsmBool HightlightStatus;

		[Tooltip("category to highlight (enum type is CubeCategory)")]
		public FsmEnum Category;

		public override void OnBegin()
		{
			ToggleCategoryHighlightNodeInputParameters inputParameter = new ToggleCategoryHighlightNodeInputParameters(HightlightStatus.get_Value(), FsmEnum.op_Implicit(Category.get_Value()));
			base.CommandsHandler.InvokeCommand(OnCommandExecutionFinished, inputParameter);
		}

		public void OnCommandExecutionFinished()
		{
			this.Finish();
		}
	}
}
