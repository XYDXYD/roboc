using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("The random probability task will return success when the random probability is above the succeed probability. It will otherwise return failure.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=33")]
	public class RandomProbability : Conditional
	{
		[Tooltip("The chance that the task will return success")]
		public SharedFloat successProbability = 0.5f;

		[Tooltip("Seed the random number generator to make things easier to debug")]
		public SharedInt seed;

		[Tooltip("Do we want to use the seed?")]
		public SharedBool useSeed;

		public RandomProbability()
			: this()
		{
		}

		public override void OnAwake()
		{
			if (useSeed.get_Value())
			{
				Random.InitState(seed.get_Value());
			}
		}

		public override TaskStatus OnUpdate()
		{
			float value = Random.get_value();
			if (!(value < successProbability.get_Value()))
			{
				return 1;
			}
			return 2;
		}

		public override void OnReset()
		{
			successProbability = 0.5f;
			seed = 0;
			useSeed = false;
		}
	}
}
