namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("Pause or disable a behavior tree and return success after it has been stopped.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=21")]
	[TaskIcon("{SkinColor}StopBehaviorTreeIcon.png")]
	public class StopBehaviorTree : Action
	{
		[Tooltip("The GameObject of the behavior tree that should be stopped. If null use the current behavior")]
		public SharedGameObject behaviorGameObject;

		[Tooltip("The group of the behavior tree that should be stopped")]
		public SharedInt group;

		[Tooltip("Should the behavior be paused or completely disabled")]
		public SharedBool pauseBehavior = false;

		private Behavior behavior;

		public StopBehaviorTree()
			: this()
		{
		}

		public override void OnStart()
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
				behavior.DisableBehavior(pauseBehavior.get_Value());
				return 2;
			}
			return 1;
		}

		public override void OnReset()
		{
			behaviorGameObject = null;
			group = 0;
			pauseBehavior = false;
		}
	}
}
