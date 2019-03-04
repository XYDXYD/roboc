using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityTransform
{
	[TaskCategory("Basic/Transform")]
	[TaskDescription("Applies a rotation. Returns Success.")]
	public class Rotate : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("Amount to rotate")]
		public SharedVector3 eulerAngles;

		[Tooltip("Specifies which axis the rotation is relative to")]
		public Space relativeTo = 1;

		private Transform targetTransform;

		private GameObject prevGameObject;

		public Rotate()
			: this()
		{
		}//IL_0002: Unknown result type (might be due to invalid IL or missing references)


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
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			if (targetTransform == null)
			{
				Debug.LogWarning((object)"Transform is null");
				return 1;
			}
			targetTransform.Rotate(eulerAngles.get_Value(), relativeTo);
			return 2;
		}

		public override void OnReset()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			targetGameObject = null;
			eulerAngles = Vector3.get_zero();
			relativeTo = 1;
		}
	}
}
