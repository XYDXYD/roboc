using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityRigidbody
{
	[TaskCategory("Basic/Rigidbody")]
	[TaskDescription("Stores the rotation of the Rigidbody. Returns Success.")]
	public class SetRotation : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The rotation of the Rigidbody")]
		public SharedQuaternion rotation;

		private Rigidbody rigidbody;

		private GameObject prevGameObject;

		public SetRotation()
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
			rigidbody.set_rotation(rotation.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			targetGameObject = null;
			rotation = Quaternion.get_identity();
		}
	}
}
