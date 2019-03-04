namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Gets the GameObject from the Transform component. Returns Success.")]
	public class SharedTransformToGameObject : Action
	{
		[Tooltip("The Transform component")]
		public SharedTransform sharedTransform;

		[RequiredField]
		[Tooltip("The GameObject to set")]
		public SharedGameObject sharedGameObject;

		public SharedTransformToGameObject()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			if (!(sharedTransform.get_Value() == null))
			{
				sharedGameObject.set_Value(sharedTransform.get_Value().get_gameObject());
				return 2;
			}
			return 1;
		}

		public override void OnReset()
		{
			sharedTransform = null;
			sharedGameObject = null;
		}
	}
}
