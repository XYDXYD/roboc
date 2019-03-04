using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityTime
{
	[TaskCategory("Basic/Time")]
	[TaskDescription("Returns the time in seconds it took to complete the last frame.")]
	public class GetDeltaTime : Action
	{
		[Tooltip("The variable to store the result")]
		public SharedFloat storeResult;

		public GetDeltaTime()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			storeResult.set_Value(Time.get_deltaTime());
			return 2;
		}

		public override void OnReset()
		{
			storeResult.set_Value(0f);
		}
	}
}
