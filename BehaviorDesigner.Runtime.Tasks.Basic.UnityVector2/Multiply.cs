using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityVector2
{
	[TaskCategory("Basic/Vector2")]
	[TaskDescription("Multiply the Vector2 by a float.")]
	public class Multiply : Action
	{
		[Tooltip("The Vector2 to multiply of")]
		public SharedVector2 vector2Variable;

		[Tooltip("The value to multiply the Vector2 of")]
		public SharedFloat multiplyBy;

		[Tooltip("The multiplication resut")]
		[RequiredField]
		public SharedVector2 storeResult;

		public Multiply()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			storeResult.set_Value(vector2Variable.get_Value() * multiplyBy.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			vector2Variable = (storeResult = Vector2.get_zero());
			multiplyBy = 0f;
		}
	}
}
