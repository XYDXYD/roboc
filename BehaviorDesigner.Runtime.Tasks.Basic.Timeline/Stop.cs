using UnityEngine;
using UnityEngine.Playables;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Timeline
{
	[TaskCategory("Basic/Timeline")]
	[TaskDescription("Stops playback of the current Playable and destroys the corresponding graph.")]
	public class Stop : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		private PlayableDirector playableDirector;

		private GameObject prevGameObject;

		public Stop()
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
			playableDirector.Stop();
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
		}
	}
}
