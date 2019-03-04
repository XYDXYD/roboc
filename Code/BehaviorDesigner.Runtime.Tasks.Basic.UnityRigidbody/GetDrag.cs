using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityRigidbody
{
	[TaskCategory("Basic/Rigidbody")]
	[TaskDescription("Stores the drag of the Rigidbody. Returns Success.")]
	public class GetDrag : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The drag of the Rigidbody")]
		[RequiredField]
		public SharedFloat storeValue;

		private Rigidbody rigidbody;

		private GameObject prevGameObject;

		public GetDrag()
			: this()
		{
		}

		public override void OnStart()
		{
			GameObject defaultGameObject = this.GetDefaultGameObject(targetGameObject.get_Value());
			if (defaultGameObject != prevGameObject)
			{
				rigidbody = defaultGameObject.GetComponent<Rigidbody>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (rigidbody == null)
			{
				Debug.LogWarning((object)"Rigidbody is null");
				return 1;
			}
			storeValue.set_Value(rigidbody.get_drag());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			storeValue = 0f;
		}
	}
}