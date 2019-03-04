using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityRigidbody2D
{
	[TaskCategory("Basic/Rigidbody2D")]
	[TaskDescription("Rotates the Rigidbody2D to the specified rotation. Returns Success.")]
	public class MoveRotation : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The new rotation of the Rigidbody")]
		public SharedFloat rotation;

		private Rigidbody2D rigidbody2D;

		private GameObject prevGameObject;

		public MoveRotation()
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
				Debug.LogWarning((object)"Rigidbody is null");
				return 1;
			}
			rigidbody2D.MoveRotation(rotation.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			rotation = 0f;
		}
	}
}
