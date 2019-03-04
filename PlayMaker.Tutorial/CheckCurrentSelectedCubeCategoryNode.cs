using HutongGames.PlayMaker;
using System;

namespace PlayMaker.Tutorial
{
	[ActionCategory(/*Could not decode attribute arguments.*/)]
	[Tooltip("TUTORIAL: check the current selected cube category, store the result in the enum.")]
	public class CheckCurrentSelectedCubeCategoryNode : PlayMakerCustomNodeBase
	{
		[Tooltip("this is set by the node to CubeCategory of the current category")]
		public FsmEnum output_currentCategory;

		[Tooltip("an optional event fired when the node finishes.")]
		public FsmEvent finished_event;

		public override void OnBegin()
		{
			IPlayMakerDataRequest dataRequest = new CheckCurrentSelectedCategoryRequest(OnRequestResultReturned);
			base.RequestsHandler.RequestData(dataRequest);
		}

		public void OnRequestResultReturned(CheckCurrentSelectedCategoryRequestReturn requestResult)
		{
			output_currentCategory.set_Value((Enum)requestResult.cubeCategorySelected);
			if (finished_event != null)
			{
				this.get_Fsm().Event(finished_event);
			}
			this.Finish();
		}
	}
}
