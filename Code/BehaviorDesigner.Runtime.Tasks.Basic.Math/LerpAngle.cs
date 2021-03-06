using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Math
{
	[TaskCategory("Basic/Math")]
	[TaskDescription("Lerp the angle by an amount.")]
	public class LerpAngle : Action
	{
		[Tooltip("The from value")]
		public SharedFloat fromValue;

		[Tooltip("The to value")]
		public SharedFloat toValue;

		[Tooltip("The amount to lerp")]
		public SharedFloat lerpAmount;

		[Tooltip("The lerp resut")]
		[RequiredField]
		public SharedFloat storeResult;

		public LerpAngle()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			storeResult.set_Value(Mathf.LerpAngle(fromValue.get_Value(), toValue.get_Value(), lerpAmount.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			fromValue = (toValue = (lerpAmount = (storeResult = 0f)));
		}
	}
}
