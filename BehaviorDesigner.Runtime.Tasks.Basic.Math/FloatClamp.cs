using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Math
{
	[TaskCategory("Basic/Math")]
	[TaskDescription("Clamps the float between two values.")]
	public class FloatClamp : Action
	{
		[Tooltip("The float to clamp")]
		public SharedFloat floatVariable;

		[Tooltip("The maximum value of the float")]
		public SharedFloat minValue;

		[Tooltip("The maximum value of the float")]
		public SharedFloat maxValue;

		public FloatClamp()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			floatVariable.set_Value(Mathf.Clamp(floatVariable.get_Value(), minValue.get_Value(), maxValue.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			floatVariable = (minValue = (maxValue = 0f));
		}
	}
}
