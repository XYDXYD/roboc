using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityRigidbody
{
	[TaskCategory("Basic/Rigidbody")]
	[TaskDescription("Sets the angular drag of the Rigidbody. Returns Success.")]
	public class SetAngularDrag : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The angular drag of the Rigidbody")]
		public SharedFloat angularDrag;

		private Rigidbody rigidbody;

		private GameObject prevGameObject;

		public SetAngularDrag()
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
			rigidbody.set_angularDrag(angularDrag.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			angularDrag = 0f;
		}
	}
}
