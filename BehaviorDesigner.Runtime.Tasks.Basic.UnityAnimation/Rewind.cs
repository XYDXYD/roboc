using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimation
{
	[TaskCategory("Basic/Animation")]
	[TaskDescription("Rewinds an animation. Rewinds all animations if animationName is blank. Returns Success.")]
	public class Rewind : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The name of the animation")]
		public SharedString animationName;

		private Animation animation;

		private GameObject prevGameObject;

		public Rewind()
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
			if (animation == null)
			{
				Debug.LogWarning((object)"Animation is null");
				return 1;
			}
			if (string.IsNullOrEmpty(animationName.get_Value()))
			{
				animation.Rewind();
			}
			else
			{
				animation.Rewind(animationName.get_Value());
			}
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			animationName.set_Value(string.Empty);
		}
	}
}
