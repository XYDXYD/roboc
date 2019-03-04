using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory("Tutorial")]
	[Tooltip("moves the build mode camera to a specific location")]
	public class AdjustCameraPositionNode : PlayMakerCustomNodeBase
	{
		[RequiredField]
		public FsmVector3 newCameraLocation;

		[RequiredField]
		public FsmVector3 newCameraOrientation;

		public override void OnBegin()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			AdjustCameraPositionNodeInputParameters inputParameter = new AdjustCameraPositionNodeInputParameters(FsmVector3.op_Implicit(newCameraLocation.get_Value()), FsmVector3.op_Implicit(newCameraOrientation.get_Value()));
			base.CommandsHandler.InvokeCommand(OnCommandExecutionFinished, inputParameter);
		}

		public void OnCommandExecutionFinished()
		{
			this.Finish();
		}
	}
}
