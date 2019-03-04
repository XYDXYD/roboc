using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityTime
{
	[TaskCategory("Basic/Time")]
	[TaskDescription("Sets the scale at which time is passing.")]
	public class SetTimeScale : Action
	{
		[Tooltip("The timescale")]
		public SharedFloat timeScale;

		public SetTimeScale()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			Time.set_timeScale(timeScale.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			timeScale.set_Value(0f);
		}
	}
}
