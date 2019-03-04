using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory("Tutorial")]
	[Tooltip("selects a specified kind of cube")]
	public class SelectSpecificTypeOfCubeNode : PlayMakerCustomNodeBase
	{
		[RequiredField]
		[Tooltip("input parameter: the cube type- this is the long string e.g. 0d8ae0c6 is the regular cube")]
		public FsmString CubeCode;

		public override void OnBegin()
		{
			SelectSpecificTypeOfCubeNodeInputParameters inputParameter = new SelectSpecificTypeOfCubeNodeInputParameters(CubeCode.get_Value());
			base.CommandsHandler.InvokeCommand(OnCommandExecutionFinished, inputParameter);
		}

		public void OnCommandExecutionFinished()
		{
			this.Finish();
		}
	}
}
