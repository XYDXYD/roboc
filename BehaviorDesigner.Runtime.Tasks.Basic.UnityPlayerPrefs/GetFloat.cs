using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityPlayerPrefs
{
	[TaskCategory("Basic/PlayerPrefs")]
	[TaskDescription("Stores the value with the specified key from the PlayerPrefs.")]
	public class GetFloat : Action
	{
		[Tooltip("The key to store")]
		public SharedString key;

		[Tooltip("The default value")]
		public SharedFloat defaultValue;

		[Tooltip("The value retrieved from the PlayerPrefs")]
		[RequiredField]
		public SharedFloat storeResult;

		public GetFloat()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			storeResult.set_Value(PlayerPrefs.GetFloat(key.get_Value(), defaultValue.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			key = string.Empty;
			defaultValue = 0f;
			storeResult = 0f;
		}
	}
}
