using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory(/*Could not decode attribute arguments.*/)]
	[Tooltip("TUTORIAL: checks has the target robot beem destroyed")]
	public class CheckTargetRobotDestroyedNode : PlayMakerCustomNodeBase
	{
		[Tooltip("this is set by the node to true or false")]
		public FsmBool output_targetdestroyed;

		[Tooltip("an optional event fired when the node finishes.")]
		public FsmEvent finished_event;

		public override void OnBegin()
		{
			IPlayMakerDataRequest dataRequest = new CheckTargetRobotDestroyedRequest(OnRequestResultReturned);
			base.RequestsHandler.RequestData(dataRequest);
		}

		public override void OnUpdate()
		{
		}

		public void OnRequestResultReturned(CheckTargetRobotDestroyedRequestReturn requestResult)
		{
			output_targetdestroyed.set_Value(requestResult.hasDestroyedTargetRobot);
			if (finished_event != null)
			{
				this.get_Fsm().Event(finished_event);
			}
			this.Finish();
		}
	}
}
