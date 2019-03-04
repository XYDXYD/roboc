using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Math
{
	[TaskCategory("Basic/Math")]
	[TaskDescription("Stores the absolute value of the float.")]
	public class FloatAbs : Action
	{
		[Tooltip("The float to return the absolute value of")]
		public SharedFloat floatVariable;

		public FloatAbs()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			floatVariable.set_Value(Mathf.Abs(floatVariable.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			floatVariable = 0f;
		}
	}
}
