using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityRigidbody
{
	[TaskCategory("Basic/Rigidbody")]
	[TaskDescription("Sets the velocity of the Rigidbody. Returns Success.")]
	public class SetVelocity : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The velocity of the Rigidbody")]
		public SharedVector3 velocity;

		private Rigidbody rigidbody;

		private GameObject prevGameObject;

		public SetVelocity()
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
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			if (rigidbody == null)
			{
				Debug.LogWarning((object)"Rigidbody is null");
				return 1;
			}
			rigidbody.set_velocity(velocity.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			targetGameObject = null;
			velocity = Vector3.get_zero();
		}
	}
}
