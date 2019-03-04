using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory("Tutorial")]
	[Tooltip("turns on or off highlighting for a specific cube type in the cube selector")]
	public class ToggleCubeHighlightingNode : PlayMakerCustomNodeBase
	{
		[Tooltip("highlight on or off")]
		[UIHint(/*Could not decode attribute arguments.*/)]
		public FsmBool HightlightCube;

		[Tooltip("Cube type hex code")]
		public FsmString CubeType;

		public override void OnBegin()
		{
			ToggleCubeHighlightingNodeInputParameters inputParameter = new ToggleCubeHighlightingNodeInputParameters(HightlightCube.get_Value(), CubeType.get_Value());
			base.CommandsHandler.InvokeCommand(OnCommandExecutionFinished, inputParameter);
		}

		public void OnCommandExecutionFinished()
		{
			this.Finish();
		}
	}
}
