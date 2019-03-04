using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory("Tutorial")]
	[Tooltip("reset tutorial inventory cubes node - this node has been deprecated now and does nothing.")]
	public class ResetTutorialInventoryCubesNode : PlayMakerCustomNodeBase
	{
		[Tooltip("optional")]
		public FsmEvent finished_event;

		public override void OnBegin()
		{
			if (finished_event != null)
			{
				this.get_Fsm().Event(finished_event);
			}
			this.Finish();
		}
	}
}
