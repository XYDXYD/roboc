using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityTransform
{
	[TaskCategory("Basic/Transform")]
	[TaskDescription("Gets the Angle between a GameObject's forward direction and a target. Returns Success.")]
	public class GetAngleToTarget : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The target object to measure the angle to. If null the targetPosition will be used.")]
		public SharedGameObject targetObject;

		[Tooltip("The world position to measure an angle to. If the targetObject is also not null, this value is used as an offset from that object's position.")]
		public SharedVector3 targetPosition;

		[Tooltip("Ignore height differences when calculating the angle?")]
		public SharedBool ignoreHeight = true;

		[Tooltip("The angle to the target")]
		[RequiredField]
		public SharedFloat storeValue;

		private Transform targetTransform;

		private GameObject prevGameObject;

		public GetAngleToTarget()
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
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			if (targetTransform == null)
			{
				Debug.LogWarning((object)"Transform is null");
				return 1;
			}
			Vector3 val = (!(targetObject.get_Value() != null)) ? targetPosition.get_Value() : targetObject.get_Value().get_transform().InverseTransformPoint(targetPosition.get_Value());
			if (ignoreHeight.get_Value())
			{
				Vector3 position = targetTransform.get_position();
				val.y = position.y;
			}
			Vector3 val2 = val - targetTransform.get_position();
			storeValue.set_Value(Vector3.Angle(val2, targetTransform.get_forward()));
			return 2;
		}

		public override void OnReset()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			targetGameObject = null;
			targetObject = null;
			targetPosition = Vector3.get_zero();
			ignoreHeight = true;
			storeValue = 0f;
		}
	}
}
