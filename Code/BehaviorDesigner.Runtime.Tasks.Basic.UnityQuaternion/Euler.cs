using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityQuaternion
{
	[TaskCategory("Basic/Quaternion")]
	[TaskDescription("Stores the quaternion of a euler vector.")]
	public class Euler : Action
	{
		[Tooltip("The euler vector")]
		public SharedVector3 eulerVector;

		[Tooltip("The stored quaternion")]
		[RequiredField]
		public SharedQuaternion storeResult;

		public Euler()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			storeResult.set_Value(Quaternion.Euler(eulerVector.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			eulerVector = Vector3.get_zero();
			storeResult = Quaternion.get_identity();
		}
	}
}
