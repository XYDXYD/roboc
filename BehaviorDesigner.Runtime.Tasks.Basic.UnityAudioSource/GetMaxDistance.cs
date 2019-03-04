using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAudioSource
{
	[TaskCategory("Basic/AudioSource")]
	[TaskDescription("Stores the max distance value of the AudioSource. Returns Success.")]
	public class GetMaxDistance : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The max distance value of the AudioSource")]
		[RequiredField]
		public SharedFloat storeValue;

		private AudioSource audioSource;

		private GameObject prevGameObject;

		public GetMaxDistance()
			: this()
		{
		}

		public override void OnStart()
		{
			GameObject defaultGameObject = this.GetDefaultGameObject(targetGameObject.get_Value());
			if (defaultGameObject != prevGameObject)
			{
				audioSource = defaultGameObject.GetComponent<AudioSource>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (audioSource == null)
			{
				Debug.LogWarning((object)"AudioSource is null");
				return 1;
			}
			storeValue.set_Value(audioSource.get_maxDistance());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			storeValue = 1f;
		}
	}
}
