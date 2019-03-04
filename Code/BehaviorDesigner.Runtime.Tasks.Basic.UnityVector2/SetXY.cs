using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityVector2
{
	[TaskCategory("Basic/Vector2")]
	[TaskDescription("Sets the X and Y values of the Vector2.")]
	public class SetXY : Action
	{
		[Tooltip("The Vector2 to set the values of")]
		public SharedVector2 vector2Variable;

		[Tooltip("The X value. Set to None to have the value ignored")]
		public SharedFloat xValue;

		[Tooltip("The Y value. Set to None to have the value ignored")]
		public SharedFloat yValue;

		public SetXY()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			Vector2 value = vector2Variable.get_Value();
			if (!xValue.get_IsNone())
			{
				value.x = xValue.get_Value();
			}
			if (!yValue.get_IsNone())
			{
				value.y = yValue.get_Value();
			}
			vector2Variable.set_Value(value);
			return 2;
		}

		public override void OnReset()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			vector2Variable = Vector2.get_zero();
			xValue = (yValue = 0f);
		}
	}
}
