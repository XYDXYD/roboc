using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityVector3
{
	[TaskCategory("Basic/Vector3")]
	[TaskDescription("Sets the X, Y, and Z values of the Vector3.")]
	public class SetXYZ : Action
	{
		[Tooltip("The Vector3 to set the values of")]
		public SharedVector3 vector3Variable;

		[Tooltip("The X value. Set to None to have the value ignored")]
		public SharedFloat xValue;

		[Tooltip("The Y value. Set to None to have the value ignored")]
		public SharedFloat yValue;

		[Tooltip("The Z value. Set to None to have the value ignored")]
		public SharedFloat zValue;

		public SetXYZ()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			Vector3 value = vector3Variable.get_Value();
			if (!xValue.get_IsNone())
			{
				value.x = xValue.get_Value();
			}
			if (!yValue.get_IsNone())
			{
				value.y = yValue.get_Value();
			}
			if (!zValue.get_IsNone())
			{
				value.z = zValue.get_Value();
			}
			vector3Variable.set_Value(value);
			return 2;
		}

		public override void OnReset()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			vector3Variable = Vector3.get_zero();
			xValue = (yValue = (zValue = 0f));
		}
	}
}
