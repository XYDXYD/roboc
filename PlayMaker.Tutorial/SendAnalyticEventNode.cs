using HutongGames.PlayMaker;

namespace PlayMaker.Tutorial
{
	[ActionCategory("Tutorial")]
	public class SendAnalyticEventNode : PlayMakerCustomNodeBase
	{
		[Tooltip("Analytics event sent")]
		public FsmString analyticsEvent;

		public override void OnBegin()
		{
			SendAnalyticEventNodeInputParameters inputParameter = new SendAnalyticEventNodeInputParameters(analyticsEvent.get_Value());
			base.CommandsHandler.InvokeCommand(OnCommandExecutionFinished, inputParameter);
		}

		public void OnCommandExecutionFinished()
		{
			this.Finish();
		}
	}
}
