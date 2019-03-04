using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityPlayerPrefs
{
	[TaskCategory("Basic/PlayerPrefs")]
	[TaskDescription("Retruns success if the specified key exists.")]
	public class HasKey : Conditional
	{
		[Tooltip("The key to check")]
		public SharedString key;

		public HasKey()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			return (!PlayerPrefs.HasKey(key.get_Value())) ? 1 : 2;
		}

		public override void OnReset()
		{
			key = string.Empty;
		}
	}
}
