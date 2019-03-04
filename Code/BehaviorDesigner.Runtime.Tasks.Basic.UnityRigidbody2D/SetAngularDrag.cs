using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityRigidbody2D
{
	[TaskCategory("Basic/Rigidbody2D")]
	[TaskDescription("Sets the angular drag of the Rigidbody2D. Returns Success.")]
	public class SetAngularDrag : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The angular drag of the Rigidbody2D")]
		public SharedFloat angularDrag;

		private Rigidbody2D rigidbody2D;

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
				rigidbody2D = defaultGameObject.GetComponent<Rigidbody2D>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (rigidbody2D == null)
			{
				Debug.LogWarning((object)"Rigidbody2D is null");
				return 1;
			}
			rigidbody2D.set_angularDrag(angularDrag.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			angularDrag = 0f;
		}
	}
}