namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("Sends an event to the behavior tree, returns success after sending the event.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=121")]
	[TaskIcon("{SkinColor}SendEventIcon.png")]
	public class SendEvent : Action
	{
		[Tooltip("The GameObject of the behavior tree that should have the event sent to it. If null use the current behavior")]
		public SharedGameObject targetGameObject;

		[Tooltip("The event to send")]
		public SharedString eventName;

		[Tooltip("The group of the behavior tree that the event should be sent to")]
		public SharedInt group;

		[Tooltip("Optionally specify a first argument to send")]
		[SharedRequired]
		public SharedVariable argument1;

		[Tooltip("Optionally specify a second argument to send")]
		[SharedRequired]
		public SharedVariable argument2;

		[Tooltip("Optionally specify a third argument to send")]
		[SharedRequired]
		public SharedVariable argument3;

		private BehaviorTree behaviorTree;

		public SendEvent()
			: this()
		{
		}

		public override void OnStart()
		{
			BehaviorTree[] components = this.GetDefaultGameObject(targetGameObject.get_Value()).GetComponents<BehaviorTree>();
			if (components.Length == 1)
			{
				behaviorTree = components[0];
			}
			else
			{
				if (components.Length <= 1)
				{
					return;
				}
				for (int i = 0; i < components.Length; i++)
				{
					if (components[i].get_Group() == group.get_Value())
					{
						behaviorTree = components[i];
						break;
					}
				}
				if (behaviorTree == null)
				{
					behaviorTree = components[0];
				}
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (argument1 == null || argument1.get_IsNone())
			{
				behaviorTree.SendEvent(eventName.get_Value());
			}
			else if (argument2 == null || argument2.get_IsNone())
			{
				behaviorTree.SendEvent<object>(eventName.get_Value(), argument1.GetValue());
			}
			else if (argument3 == null || argument3.get_IsNone())
			{
				behaviorTree.SendEvent<object, object>(eventName.get_Value(), argument1.GetValue(), argument2.GetValue());
			}
			else
			{
				behaviorTree.SendEvent<object, object, object>(eventName.get_Value(), argument1.GetValue(), argument2.GetValue(), argument3.GetValue());
			}
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			eventName = string.Empty;
		}
	}
}
