using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityQuaternion
{
	[TaskCategory("Basic/Quaternion")]
	[TaskDescription("Stores the quaternion of a forward vector.")]
	public class LookRotation : Action
	{
		[Tooltip("The forward vector")]
		public SharedVector3 forwardVector;

		[Tooltip("The second Vector3")]
		public SharedVector3 secondVector3;

		[Tooltip("The stored quaternion")]
		[RequiredField]
		public SharedQuaternion storeResult;

		public LookRotation()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			storeResult.set_Value(Quaternion.LookRotation(forwardVector.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			forwardVector = Vector3.get_zero();
			storeResult = Quaternion.get_identity();
		}
	}
}
