using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityQuaternion
{
	[TaskCategory("Basic/Quaternion")]
	[TaskDescription("Stores the inverse of the specified quaternion.")]
	public class Inverse : Action
	{
		[Tooltip("The target quaternion")]
		public SharedQuaternion targetQuaternion;

		[Tooltip("The stored quaternion")]
		[RequiredField]
		public SharedQuaternion storeResult;

		public Inverse()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			storeResult.set_Value(Quaternion.Inverse(targetQuaternion.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			targetQuaternion = (storeResult = Quaternion.get_identity());
		}
	}
}
