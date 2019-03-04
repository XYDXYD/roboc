using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimation
{
	[TaskCategory("Basic/Animation")]
	[TaskDescription("Plays animation without any blending. Returns Success.")]
	public class Play : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The name of the animation")]
		public SharedString animationName;

		[Tooltip("The play mode of the animation")]
		public PlayMode playMode;

		private Animation animation;

		private GameObject prevGameObject;

		public Play()
			: this()
		{
		}

		public override void OnStart()
		{
			GameObject defaultGameObject = this.GetDefaultGameObject(targetGameObject.get_Value());
			if (defaultGameObject != prevGameObject)
			{
				animation = defaultGameObject.GetComponent<Animation>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			if (animation == null)
			{
				Debug.LogWarning((object)"Animation is null");
				return 1;
			}
			if (string.IsNullOrEmpty(animationName.get_Value()))
			{
				animation.Play();
			}
			else
			{
				animation.Play(animationName.get_Value(), playMode);
			}
			return 2;
		}

		public override void OnReset()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			targetGameObject = null;
			animationName.set_Value(string.Empty);
			playMode = 0;
		}
	}
}
