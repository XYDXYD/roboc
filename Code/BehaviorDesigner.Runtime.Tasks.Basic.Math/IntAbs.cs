using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Math
{
	[TaskCategory("Basic/Math")]
	[TaskDescription("Stores the absolute value of the int.")]
	public class IntAbs : Action
	{
		[Tooltip("The int to return the absolute value of")]
		public SharedInt intVariable;

		public IntAbs()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			intVariable.set_Value(Mathf.Abs(intVariable.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			intVariable = 0;
		}
	}
}
