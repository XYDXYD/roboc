using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAudioSource
{
	[TaskCategory("Basic/AudioSource")]
	[TaskDescription("Sets the clip value of the AudioSource. Returns Success.")]
	public class SetAudioClip : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The AudioSource clip")]
		public AudioClip audioClip;

		private AudioSource audioSource;

		private GameObject prevGameObject;

		public SetAudioClip()
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
			audioSource.set_clip(audioClip);
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			audioClip = null;
		}
	}
}
