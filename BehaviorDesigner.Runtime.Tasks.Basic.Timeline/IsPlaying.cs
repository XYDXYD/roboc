using UnityEngine;
using UnityEngine.Playables;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Timeline
{
	[TaskCategory("Basic/Timeline")]
	[TaskDescription("Is the timeline currently playing?")]
	public class IsPlaying : Conditional
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		private PlayableDirector playableDirector;

		private GameObject prevGameObject;

		public IsPlaying()
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
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Invalid comparison between Unknown and I4
			if (playableDirector == null)
			{
				Debug.LogWarning((object)"PlayableDirector is null");
				return 1;
			}
			return ((int)playableDirector.get_state() != 1) ? 1 : 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
		}
	}
}
