using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory("Tutorial")]
	[Tooltip("saves current tutorial bay")]
	public class SaveTutorialBayNode : PlayMakerCustomNodeBase
	{
		[Tooltip("optional")]
		public FsmEvent finished_event;

		public override void OnBegin()
		{
			SaveTutorialBayPlaymakerRequest dataRequest = new SaveTutorialBayPlaymakerRequest(OnRequestResultReturned);
			base.RequestsHandler.RequestData(dataRequest);
		}

		public void OnRequestResultReturned(SaveTutorialBayPlaymakerRequestReturn requestResult)
		{
			if (finished_event != null)
			{
				this.get_Fsm().Event(finished_event);
			}
			this.Finish();
		}
	}
}
