using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityTime
{
	[TaskCategory("Basic/Time")]
	[TaskDescription("Returns the scale at which time is passing.")]
	public class GetTimeScale : Action
	{
		[Tooltip("The variable to store the result")]
		public SharedFloat storeResult;

		public GetTimeScale()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			storeResult.set_Value(Time.get_timeScale());
			return 2;
		}

		public override void OnReset()
		{
			storeResult.set_Value(0f);
		}
	}
}
