using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimation
{
	[TaskCategory("Basic/Animation")]
	[TaskDescription("Sets animate physics to the specified value. Returns Success.")]
	public class SetAnimatePhysics : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("Are animations executed in the physics loop?")]
		public SharedBool animatePhysics;

		private Animation animation;

		private GameObject prevGameObject;

		public SetAnimatePhysics()
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
			animation.set_animatePhysics(animatePhysics.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			animatePhysics.set_Value(false);
		}
	}
}
