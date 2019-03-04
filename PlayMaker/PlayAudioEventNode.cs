using HutongGames.PlayMaker;

namespace PlayMaker
{
	[ActionCategory(/*Could not decode attribute arguments.*/)]
	[Tooltip("PLAY SOUND: this node plays an arbitrary audio event")]
	public class PlayAudioEventNode : PlayMakerCustomNodeBase
	{
		[Tooltip("audio event to be played")]
		[UIHint(/*Could not decode attribute arguments.*/)]
		public FsmString audioEvent;

		public FsmEnum eventType;

		public override void OnBegin()
		{
			PlayAudioEventNodeCommandParameters inputParameter = new PlayAudioEventNodeCommandParameters(audioEvent, eventType);
			base.CommandsHandler.InvokeCommand(OnCommandExecutionFinished, inputParameter);
		}

		public void OnCommandExecutionFinished()
		{
			this.Finish();
		}
	}
}
