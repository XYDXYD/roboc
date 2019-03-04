namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("Restarts a behavior tree, returns success after it has been restarted.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=66")]
	[TaskIcon("{SkinColor}RestartBehaviorTreeIcon.png")]
	public class RestartBehaviorTree : Action
	{
		[Tooltip("The GameObject of the behavior tree that should be restarted. If null use the current behavior")]
		public SharedGameObject behaviorGameObject;

		[Tooltip("The group of the behavior tree that should be restarted")]
		public SharedInt group;

		private Behavior behavior;

		public RestartBehaviorTree()
			: this()
		{
		}

		public override void OnAwake()
		{
			Behavior[] components = this.GetDefaultGameObject(behaviorGameObject.get_Value()).GetComponents<Behavior>();
			if (components.Length == 1)
			{
				behavior = components[0];
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
						behavior = components[i];
						break;
					}
				}
				if (behavior == null)
				{
					behavior = components[0];
				}
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (!(behavior == null))
			{
				behavior.DisableBehavior();
				behavior.EnableBehavior();
				return 2;
			}
			return 1;
		}

		public override void OnReset()
		{
			behavior = null;
		}
	}
}
