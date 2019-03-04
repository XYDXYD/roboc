using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAudioSource
{
	[TaskCategory("Basic/AudioSource")]
	[TaskDescription("Plays the audio clip with a delay specified in seconds. Returns Success.")]
	public class PlayDelayed : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("Delay time specified in seconds")]
		public SharedFloat delay = 0f;

		private AudioSource audioSource;

		private GameObject prevGameObject;

		public PlayDelayed()
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
			audioSource.PlayDelayed(delay.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			delay = 0f;
		}
	}
}
