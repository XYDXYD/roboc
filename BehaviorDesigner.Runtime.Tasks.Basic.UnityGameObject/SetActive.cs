namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityGameObject
{
	[TaskCategory("Basic/GameObject")]
	[TaskDescription("Activates/Deactivates the GameObject. Returns Success.")]
	public class SetActive : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("Active state of the GameObject")]
		public SharedBool active;

		public SetActive()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			this.GetDefaultGameObject(targetGameObject.get_Value()).SetActive(active.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			active = false;
		}
	}
}
