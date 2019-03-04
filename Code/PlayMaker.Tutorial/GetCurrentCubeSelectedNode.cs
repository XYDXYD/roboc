using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory("Tutorial")]
	[Tooltip("check which cube was selected")]
	public class GetCurrentCubeSelectedNode : PlayMakerCustomNodeBase
	{
		[Tooltip("cube type will go here, in string format, e.g: 0d8ae0c6 = regular cube")]
		public FsmString output_CubeCode;

		public override void OnBegin()
		{
			IPlayMakerDataRequest dataRequest = new GetCurrentCubeSelectedPlaymakerRequest(OnRequestResultReturned);
			base.RequestsHandler.RequestData(dataRequest);
		}

		public void OnRequestResultReturned(GetCurrentCubeSelectedPlaymakerRequestResult requestResult)
		{
			string currentCubeSelected = requestResult.currentCubeSelected;
			output_CubeCode.set_Value(currentCubeSelected);
			this.Finish();
		}
	}
}
