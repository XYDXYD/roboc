using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAudioSource
{
	[TaskCategory("Basic/AudioSource")]
	[TaskDescription("Sets the ignore listener pause value of the AudioSource. Returns Success.")]
	public class SetIgnoreListenerPause : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The ignore listener pause value of the AudioSource")]
		public SharedBool ignoreListenerPause;

		private AudioSource audioSource;

		private GameObject prevGameObject;

		public SetIgnoreListenerPause()
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
			audioSource.set_ignoreListenerPause(ignoreListenerPause.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			ignoreListenerPause = false;
		}
	}
}
