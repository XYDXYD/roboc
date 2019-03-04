using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Math
{
	[TaskCategory("Basic/Math")]
	[TaskDescription("Sets a random int value")]
	public class RandomInt : Action
	{
		[Tooltip("The minimum amount")]
		public SharedInt min;

		[Tooltip("The maximum amount")]
		public SharedInt max;

		[Tooltip("Is the maximum value inclusive?")]
		public bool inclusive;

		[Tooltip("The variable to store the result")]
		public SharedInt storeResult;

		public RandomInt()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			if (inclusive)
			{
				storeResult.set_Value(Random.Range(min.get_Value(), max.get_Value() + 1));
			}
			else
			{
				storeResult.set_Value(Random.Range(min.get_Value(), max.get_Value()));
			}
			return 2;
		}

		public override void OnReset()
		{
			min.set_Value(0);
			max.set_Value(0);
			inclusive = false;
			storeResult.set_Value(0);
		}
	}
}
