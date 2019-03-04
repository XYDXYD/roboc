using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityPlayerPrefs
{
	[TaskCategory("Basic/PlayerPrefs")]
	[TaskDescription("Stores the value with the specified key from the PlayerPrefs.")]
	public class GetString : Action
	{
		[Tooltip("The key to store")]
		public SharedString key;

		[Tooltip("The default value")]
		public SharedString defaultValue;

		[Tooltip("The value retrieved from the PlayerPrefs")]
		[RequiredField]
		public SharedString storeResult;

		public GetString()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			storeResult.set_Value(PlayerPrefs.GetString(key.get_Value(), defaultValue.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			key = string.Empty;
			defaultValue = string.Empty;
			storeResult = string.Empty;
		}
	}
}
