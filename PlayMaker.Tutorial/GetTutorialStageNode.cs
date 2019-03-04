using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory(/*Could not decode attribute arguments.*/)]
	[Tooltip("TUTORIAL: gets the current tutorial stage number")]
	public class GetTutorialStageNode : PlayMakerCustomNodeBase
	{
		public FsmInt output_stage;

		[Tooltip("optional")]
		public FsmEvent finished_event;

		public override void OnBegin()
		{
			IPlayMakerDataRequest dataRequest = new GetTutorialStagePlaymakerRequest(OnRequestResultReturned);
			base.RequestsHandler.RequestData(dataRequest);
		}

		public override void OnUpdate()
		{
		}

		public void OnRequestResultReturned(GetTutorialStagePlaymakerRequestResult requestResult)
		{
			int stage = requestResult.Stage;
			output_stage.set_Value(stage);
			if (finished_event != null)
			{
				this.get_Fsm().Event(finished_event);
			}
			this.Finish();
		}
	}
}
