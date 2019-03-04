using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAudioSource
{
	[TaskCategory("Basic/AudioSource")]
	[TaskDescription("Sets the pitch value of the AudioSource. Returns Success.")]
	public class SetPitch : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The pitch value of the AudioSource")]
		public SharedFloat pitch;

		private AudioSource audioSource;

		private GameObject prevGameObject;

		public SetPitch()
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
			audioSource.set_pitch(pitch.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			pitch = 1f;
		}
	}
}
