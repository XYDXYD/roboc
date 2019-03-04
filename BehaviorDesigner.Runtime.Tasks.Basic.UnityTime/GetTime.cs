using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityTime
{
	[TaskCategory("Basic/Time")]
	[TaskDescription("Returns the time in second since the start of the game.")]
	public class GetTime : Action
	{
		[Tooltip("The variable to store the result")]
		public SharedFloat storeResult;

		public GetTime()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			storeResult.set_Value(Time.get_time());
			return 2;
		}

		public override void OnReset()
		{
			storeResult.set_Value(0f);
		}
	}
}
