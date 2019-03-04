using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory(/*Could not decode attribute arguments.*/)]
	[Tooltip("TUTORIAL: checks has filled all cube locations specified in the setup node")]
	public class CheckHasFilledAllCubeLocationsNode : PlayMakerCustomNodeBase
	{
		[Tooltip("this is set by the node to true or false")]
		public FsmBool output_hasFilledCubeLocations;

		[Tooltip("an optional event fired when the node finishes.")]
		public FsmEvent finished_event;

		public override void OnBegin()
		{
			IPlayMakerDataRequest dataRequest = new CheckHasFilledAllCubeLocationsRequest(OnRequestResultReturned);
			base.RequestsHandler.RequestData(dataRequest);
		}

		public override void OnUpdate()
		{
		}

		public void OnRequestResultReturned(CheckHasFilledAllCubeLocationsRequestReturn requestResult)
		{
			output_hasFilledCubeLocations.set_Value(requestResult.HasFilledAllLocations);
			if (finished_event != null)
			{
				this.get_Fsm().Event(finished_event);
			}
			this.Finish();
		}
	}
}
