using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityVector2
{
	[TaskCategory("Basic/Vector2")]
	[TaskDescription("Move from the current position to the target position.")]
	public class MoveTowards : Action
	{
		[Tooltip("The current position")]
		public SharedVector2 currentPosition;

		[Tooltip("The target position")]
		public SharedVector2 targetPosition;

		[Tooltip("The movement speed")]
		public SharedFloat speed;

		[Tooltip("The move resut")]
		[RequiredField]
		public SharedVector2 storeResult;

		public MoveTowards()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			storeResult.set_Value(Vector2.MoveTowards(currentPosition.get_Value(), targetPosition.get_Value(), speed.get_Value() * Time.get_deltaTime()));
			return 2;
		}

		public override void OnReset()
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			currentPosition = (targetPosition = (storeResult = Vector2.get_zero()));
			speed = 0f;
		}
	}
}
