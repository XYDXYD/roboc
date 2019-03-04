namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityGameObject
{
	[TaskCategory("Basic/GameObject")]
	[TaskDescription("Sends a message to the target GameObject. Returns Success.")]
	public class SendMessage : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The message to send")]
		public SharedString message;

		[Tooltip("The value to send")]
		public SharedGenericVariable value;

		public SendMessage()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			if (value.get_Value() != null)
			{
				this.GetDefaultGameObject(targetGameObject.get_Value()).SendMessage(message.get_Value(), value.get_Value().value.GetValue());
			}
			else
			{
				this.GetDefaultGameObject(targetGameObject.get_Value()).SendMessage(message.get_Value());
			}
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			message = string.Empty;
		}
	}
}
