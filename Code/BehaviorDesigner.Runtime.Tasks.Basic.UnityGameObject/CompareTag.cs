namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityGameObject
{
	[TaskCategory("Basic/GameObject")]
	[TaskDescription("Returns Success if tags match, otherwise Failure.")]
	public class CompareTag : Conditional
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The tag to compare against")]
		public SharedString tag;

		public CompareTag()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			return (!this.GetDefaultGameObject(targetGameObject.get_Value()).CompareTag(tag.get_Value())) ? 1 : 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			tag = string.Empty;
		}
	}
}
