using UnityEngine;
using UnityEngine.Playables;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Timeline
{
	[TaskCategory("Basic/Timeline")]
	[TaskDescription("Pauses playback of the currently running playable.")]
	public class Pause : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		private PlayableDirector playableDirector;

		private GameObject prevGameObject;

		public Pause()
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
		}

		public override TaskStatus OnUpdate()
		{
			if (playableDirector == null)
			{
				Debug.LogWarning((object)"PlayableDirector is null");
				return 1;
			}
			playableDirector.Pause();
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
		}
	}
}
