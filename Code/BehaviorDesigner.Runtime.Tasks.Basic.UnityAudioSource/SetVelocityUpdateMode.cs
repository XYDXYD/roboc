using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAudioSource
{
	[TaskCategory("Basic/AudioSource")]
	[TaskDescription("Sets the rolloff mode of the AudioSource. Returns Success.")]
	public class SetVelocityUpdateMode : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The velocity update mode of the AudioSource")]
		public AudioVelocityUpdateMode velocityUpdateMode;

		private AudioSource audioSource;

		private GameObject prevGameObject;

		public SetVelocityUpdateMode()
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
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			if (audioSource == null)
			{
				Debug.LogWarning((object)"AudioSource is null");
				return 1;
			}
			audioSource.set_velocityUpdateMode(velocityUpdateMode);
			return 2;
		}

		public override void OnReset()
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			targetGameObject = null;
			velocityUpdateMode = 0;
		}
	}
}
