using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory("Tutorial")]
	[Tooltip("saves current tutorial stage")]
	public class SaveTutorialStageNode : PlayMakerCustomNodeBase
	{
		public FsmInt input_stage;

		[Tooltip("optional")]
		public FsmEvent finished_event;

		public override void OnBegin()
		{
			SaveTutorialStagePlaymakerRequest dataRequest = new SaveTutorialStagePlaymakerRequest(new SaveTutorialStagePlaymakerRequestInputParameters(input_stage.get_Value()), OnRequestResultReturned);
			base.RequestsHandler.RequestData(dataRequest);
		}

		public void OnRequestResultReturned(SaveTutorialStagePlaymakerRequestReturn requestResult)
		{
			if (finished_event != null)
			{
				this.get_Fsm().Event(finished_event);
			}
			this.Finish();
		}
	}
}
