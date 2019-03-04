using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityVector2
{
	[TaskCategory("Basic/Vector2")]
	[TaskDescription("Clamps the magnitude of the Vector2.")]
	public class ClampMagnitude : Action
	{
		[Tooltip("The Vector2 to clamp the magnitude of")]
		public SharedVector2 vector2Variable;

		[Tooltip("The max length of the magnitude")]
		public SharedFloat maxLength;

		[Tooltip("The clamp magnitude resut")]
		[RequiredField]
		public SharedVector2 storeResult;

		public ClampMagnitude()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			storeResult.set_Value(Vector2.ClampMagnitude(vector2Variable.get_Value(), maxLength.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			vector2Variable = (storeResult = Vector2.get_zero());
			maxLength = 0f;
		}
	}
}
