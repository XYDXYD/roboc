using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityRigidbody2D
{
	[TaskCategory("Basic/Rigidbody2D")]
	[TaskDescription("Applies a force at the specified position to the Rigidbody2D. Returns Success.")]
	public class AddForceAtPosition : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The amount of force to apply")]
		public SharedVector2 force;

		[Tooltip("The position of the force")]
		public SharedVector2 position;

		private Rigidbody2D rigidbody2D;

		private GameObject prevGameObject;

		public AddForceAtPosition()
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
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			if (rigidbody2D == null)
			{
				Debug.LogWarning((object)"Rigidbody2D is null");
				return 1;
			}
			rigidbody2D.AddForceAtPosition(force.get_Value(), position.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			targetGameObject = null;
			force = Vector2.get_zero();
			position = Vector2.get_zero();
		}
	}
}
