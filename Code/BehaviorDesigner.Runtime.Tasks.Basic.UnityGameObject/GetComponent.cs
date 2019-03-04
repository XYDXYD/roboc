namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityGameObject
{
	[TaskCategory("Basic/GameObject")]
	[TaskDescription("Returns the component of Type type if the game object has one attached, null if it doesn't. Returns Success.")]
	public class GetComponent : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The type of component")]
		public SharedString type;

		[Tooltip("The component")]
		[RequiredField]
		public SharedObject storeValue;

		public GetComponent()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			storeValue.set_Value(this.GetDefaultGameObject(targetGameObject.get_Value()).GetComponent(type.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			type.set_Value(string.Empty);
			storeValue.set_Value(null);
		}
	}
}
