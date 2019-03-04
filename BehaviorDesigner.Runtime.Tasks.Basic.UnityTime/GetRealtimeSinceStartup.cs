using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityTime
{
	[TaskCategory("Basic/Time")]
	[TaskDescription("Returns the real time in seconds since the game started.")]
	public class GetRealtimeSinceStartup : Action
	{
		[Tooltip("The variable to store the result")]
		public SharedFloat storeResult;

		public GetRealtimeSinceStartup()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			storeResult.set_Value(Time.get_realtimeSinceStartup());
			return 2;
		}

		public override void OnReset()
		{
			storeResult.set_Value(0f);
		}
	}
}
