using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityTransform
{
	[TaskCategory("Basic/Transform")]
	[TaskDescription("Sets the rotation of the Transform. Returns Success.")]
	public class SetRotation : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The rotation of the Transform")]
		public SharedQuaternion rotation;

		private Transform targetTransform;

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
				targetTransform = defaultGameObject.GetComponent<Transform>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			if (targetTransform == null)
			{
				Debug.LogWarning((object)"Transform is null");
				return 1;
			}
			targetTransform.set_rotation(rotation.get_Value());
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