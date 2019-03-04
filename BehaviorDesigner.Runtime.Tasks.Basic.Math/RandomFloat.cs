using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Math
{
	[TaskCategory("Basic/Math")]
	[TaskDescription("Sets a random float value")]
	public class RandomFloat : Action
	{
		[Tooltip("The minimum amount")]
		public SharedFloat min;

		[Tooltip("The maximum amount")]
		public SharedFloat max;

		[Tooltip("Is the maximum value inclusive?")]
		public bool inclusive;

		[Tooltip("The variable to store the result")]
		public SharedFloat storeResult;

		public RandomFloat()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			if (inclusive)
			{
				storeResult.set_Value(Random.Range(min.get_Value(), max.get_Value()));
			}
			else
			{
				storeResult.set_Value(Random.Range(min.get_Value(), max.get_Value() - 1E-05f));
			}
			return 2;
		}

		public override void OnReset()
		{
			min.set_Value(0f);
			max.set_Value(0f);
			inclusive = false;
			storeResult.set_Value(0f);
		}
	}
}
