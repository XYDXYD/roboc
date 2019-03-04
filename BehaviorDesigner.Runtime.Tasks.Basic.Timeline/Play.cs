using UnityEngine;
using UnityEngine.Playables;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Timeline
{
	[TaskCategory("Basic/Timeline")]
	[TaskDescription("Instatiates a Playable using the provided PlayableAsset and starts playback.")]
	public class Play : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("An asset to instantiate a playable from.")]
		public PlayableAsset playableAsset;

		[Tooltip("Should the task be stopped when the timeline has stopped playing?")]
		public SharedBool stopWhenComplete;

		private PlayableDirector playableDirector;

		private GameObject prevGameObject;

		private bool playbackStarted;

		public Play()
			: this()
		{
		}

		public override void OnStart()
		{
			GameObject defaultGameObject = this.GetDefaultGameObject(targetGameObject.get_Value());
			if (defaultGameObject != prevGameObject)
			{
				playableDirector = defaultGameObject.GetComponent<PlayableDirector>();
				prevGameObject = defaultGameObject;
			}
			playbackStarted = false;
		}

		public override TaskStatus OnUpdate()
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Invalid comparison between Unknown and I4
			if (playableDirector == null)
			{
				Debug.LogWarning((object)"PlayableDirector is null");
				return 1;
			}
			if (playbackStarted)
			{
				if (stopWhenComplete.get_Value() && (int)playableDirector.get_state() == 1)
				{
					return 3;
				}
				return 2;
			}
			if (playableAsset == null)
			{
				playableDirector.Play();
			}
			else
			{
				playableDirector.Play(playableAsset);
			}
			playbackStarted = true;
			return (!stopWhenComplete.get_Value()) ? 2 : 3;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			playableAsset = null;
			stopWhenComplete = false;
		}
	}
}
