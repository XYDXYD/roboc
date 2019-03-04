using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Math
{
	[TaskCategory("Basic/Math")]
	[TaskDescription("Sets a random bool value")]
	public class RandomBool : Action
	{
		[Tooltip("The variable to store the result")]
		public SharedBool storeResult;

		public RandomBool()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			storeResult.set_Value(Random.get_value() < 0.5f);
			return 2;
		}

		public override void OnReset()
		{
			storeResult.set_Value(false);
		}
	}
}
