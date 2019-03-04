using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimation
{
	[TaskCategory("Basic/Animation")]
	[TaskDescription("Fades the animation over a period of time and fades other animations out. Returns Success.")]
	public class CrossFade : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The name of the animation")]
		public SharedString animationName;

		[Tooltip("The amount of time it takes to blend")]
		public float fadeLength = 0.3f;

		[Tooltip("The play mode of the animation")]
		public PlayMode playMode;

		private Animation animation;

		private GameObject prevGameObject;

		public CrossFade()
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
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			if (animation == null)
			{
				Debug.LogWarning((object)"Animation is null");
				return 1;
			}
			animation.CrossFade(animationName.get_Value(), fadeLength, playMode);
			return 2;
		}

		public override void OnReset()
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			targetGameObject = null;
			animationName.set_Value(string.Empty);
			fadeLength = 0.3f;
			playMode = 0;
		}
	}
}
