using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory("Tutorial")]
	[Tooltip("Sets boolean flags for which screens are currently active")]
	public class CheckActiveScreensNode : PlayMakerCustomNodeBase
	{
		[Tooltip("output: whether the inventory screen is active or not.")]
		public FsmBool inventoryScreenActive;

		[Tooltip("Repeat every frame while the state is active. you should set this true.")]
		public bool everyFrame;

		public override void OnBegin()
		{
		}

		public override void OnUpdate()
		{
			CheckActiveScreensRequest dataRequest = new CheckActiveScreensRequest(OnRequestResultReturned);
			base.RequestsHandler.RequestData(dataRequest);
		}

		public void OnRequestResultReturned(CheckActiveScreensRequestReturn requestResult)
		{
			inventoryScreenActive.set_Value(requestResult.InventoryScreenActive);
			if (!everyFrame)
			{
				this.Finish();
			}
		}
	}
}
