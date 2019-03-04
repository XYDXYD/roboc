using UnityEngine;
using UnityEngine.Playables;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Timeline
{
	[TaskCategory("Basic/Timeline")]
	[TaskDescription("Resume playing a paused playable.")]
	public class Resume : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("Should the task be stopped when the timeline has stopped playing?")]
		public SharedBool stopWhenComplete;

		private PlayableDirector playableDirector;

		private GameObject prevGameObject;

		private bool playbackStarted;

		public Resume()
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
			playableDirector.Resume();
			playbackStarted = true;
			return (!stopWhenComplete.get_Value()) ? 2 : 3;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			stopWhenComplete = false;
		}
	}
}
