using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityQuaternion
{
	[TaskCategory("Basic/Quaternion")]
	[TaskDescription("Stores a rotation which rotates from the first direction to the second.")]
	public class FromToRotation : Action
	{
		[Tooltip("The from rotation")]
		public SharedVector3 fromDirection;

		[Tooltip("The to rotation")]
		public SharedVector3 toDirection;

		[Tooltip("The stored result")]
		[RequiredField]
		public SharedQuaternion storeResult;

		public FromToRotation()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			storeResult.set_Value(Quaternion.FromToRotation(fromDirection.get_Value(), toDirection.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			fromDirection = (toDirection = Vector3.get_zero());
			storeResult = Quaternion.get_identity();
		}
	}
}
