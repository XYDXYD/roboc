using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityTransform
{
	[TaskCategory("Basic/Transform")]
	[TaskDescription("Applies a rotation. Returns Success.")]
	public class RotateAround : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("Point to rotate around")]
		public SharedVector3 point;

		[Tooltip("Axis to rotate around")]
		public SharedVector3 axis;

		[Tooltip("Amount to rotate")]
		public SharedFloat angle;

		private Transform targetTransform;

		private GameObject prevGameObject;

		public RotateAround()
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
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			if (targetTransform == null)
			{
				Debug.LogWarning((object)"Transform is null");
				return 1;
			}
			targetTransform.RotateAround(point.get_Value(), axis.get_Value(), angle.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			targetGameObject = null;
			point = Vector3.get_zero();
			axis = Vector3.get_zero();
			angle = 0f;
		}
	}
}
