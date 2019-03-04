using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory(/*Could not decode attribute arguments.*/)]
	[Tooltip("TUTORIAL: check if context is test mode")]
	public class CheckIsContextTestModeNode : PlayMakerCustomNodeBase
	{
		[Tooltip("fired if test node")]
		public FsmEvent is_test_mode_event;

		[Tooltip("fired if not test node")]
		public FsmEvent is_not_test_mode_event;

		public override void OnBegin()
		{
			IPlayMakerDataRequest dataRequest = new CheckInTestModeContextRequest(OnRequestResultReturned);
			base.RequestsHandler.RequestData(dataRequest);
		}

		public override void OnUpdate()
		{
		}

		public void OnRequestResultReturned(CheckInTestModeContextRequestReturn requestResult)
		{
			if (requestResult.isInTestContext)
			{
				this.get_Fsm().Event(is_test_mode_event);
			}
			else
			{
				this.get_Fsm().Event(is_not_test_mode_event);
			}
			this.Finish();
		}
	}
}
