using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Math
{
	[TaskCategory("Basic/Math")]
	[TaskDescription("Clamps the int between two values.")]
	public class IntClamp : Action
	{
		[Tooltip("The int to clamp")]
		public SharedInt intVariable;

		[Tooltip("The maximum value of the int")]
		public SharedInt minValue;

		[Tooltip("The maximum value of the int")]
		public SharedInt maxValue;

		public IntClamp()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			intVariable.set_Value(Mathf.Clamp(intVariable.get_Value(), minValue.get_Value(), maxValue.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			intVariable = (minValue = (maxValue = 0));
		}
	}
}
