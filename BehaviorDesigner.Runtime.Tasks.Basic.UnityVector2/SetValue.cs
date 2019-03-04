using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityVector2
{
	[TaskCategory("Basic/Vector2")]
	[TaskDescription("Sets the value of the Vector2.")]
	public class SetValue : Action
	{
		[Tooltip("The Vector2 to get the values of")]
		public SharedVector2 vector2Value;

		[Tooltip("The Vector2 to set the values of")]
		public SharedVector2 vector2Variable;

		public SetValue()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			vector2Variable.set_Value(vector2Value.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			vector2Value = (vector2Variable = Vector2.get_zero());
		}
	}
}
