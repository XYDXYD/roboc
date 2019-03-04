using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityPlayerPrefs
{
	[TaskCategory("Basic/PlayerPrefs")]
	[TaskDescription("Saves the PlayerPrefs.")]
	public class Save : Action
	{
		public Save()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			PlayerPrefs.Save();
			return 2;
		}
	}
}
