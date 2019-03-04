using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityQuaternion
{
	[TaskCategory("Basic/Quaternion")]
	[TaskDescription("Lerps between two quaternions.")]
	public class Lerp : Action
	{
		[Tooltip("The from rotation")]
		public SharedQuaternion fromQuaternion;

		[Tooltip("The to rotation")]
		public SharedQuaternion toQuaternion;

		[Tooltip("The amount to lerp")]
		public SharedFloat amount;

		[Tooltip("The stored result")]
		[RequiredField]
		public SharedQuaternion storeResult;

		public Lerp()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			storeResult.set_Value(Quaternion.Lerp(fromQuaternion.get_Value(), toQuaternion.get_Value(), amount.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			fromQuaternion = (toQuaternion = (storeResult = Quaternion.get_identity()));
			amount = 0f;
		}
	}
}
