namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityGameObject
{
	[TaskCategory("Basic/GameObject")]
	[TaskDescription("Sets the GameObject tag. Returns Success.")]
	public class SetTag : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The GameObject tag")]
		public SharedString tag;

		public SetTag()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			this.GetDefaultGameObject(targetGameObject.get_Value()).set_tag(tag.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			tag = string.Empty;
		}
	}
}