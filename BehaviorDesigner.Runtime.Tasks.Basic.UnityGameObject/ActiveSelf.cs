namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityGameObject
{
	[TaskCategory("Basic/GameObject")]
	[TaskDescription("Returns Success if the GameObject is active in the hierarchy, otherwise Failure.")]
	public class ActiveSelf : Conditional
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		public ActiveSelf()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			return (!this.GetDefaultGameObject(targetGameObject.get_Value()).get_activeSelf()) ? 1 : 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
		}
	}
}
