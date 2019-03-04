using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityVector3
{
	[TaskCategory("Basic/Vector3")]
	[TaskDescription("Lerp the Vector3 by an amount.")]
	public class Lerp : Action
	{
		[Tooltip("The from value")]
		public SharedVector3 fromVector3;

		[Tooltip("The to value")]
		public SharedVector3 toVector3;

		[Tooltip("The amount to lerp")]
		public SharedFloat lerpAmount;

		[Tooltip("The lerp resut")]
		[RequiredField]
		public SharedVector3 storeResult;

		public Lerp()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			storeResult.set_Value(Vector3.Lerp(fromVector3.get_Value(), toVector3.get_Value(), lerpAmount.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			fromVector3 = (toVector3 = (storeResult = Vector3.get_zero()));
			lerpAmount = 0f;
		}
	}
}
