using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityVector2
{
	[TaskCategory("Basic/Vector2")]
	[TaskDescription("Lerp the Vector2 by an amount.")]
	public class Lerp : Action
	{
		[Tooltip("The from value")]
		public SharedVector2 fromVector2;

		[Tooltip("The to value")]
		public SharedVector2 toVector2;

		[Tooltip("The amount to lerp")]
		public SharedFloat lerpAmount;

		[Tooltip("The lerp resut")]
		[RequiredField]
		public SharedVector2 storeResult;

		public Lerp()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			storeResult.set_Value(Vector2.Lerp(fromVector2.get_Value(), toVector2.get_Value(), lerpAmount.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			fromVector2 = (toVector2 = (storeResult = Vector2.get_zero()));
			lerpAmount = 0f;
		}
	}
}
