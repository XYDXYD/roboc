using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityDebug
{
	[TaskCategory("Basic/Debug")]
	[TaskDescription("Log a variable value.")]
	public class LogValue : Action
	{
		[Tooltip("The variable to output")]
		public SharedGenericVariable variable;

		public LogValue()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			Debug.Log(variable.get_Value().value.GetValue());
			return 2;
		}

		public override void OnReset()
		{
			variable = null;
		}
	}
}
