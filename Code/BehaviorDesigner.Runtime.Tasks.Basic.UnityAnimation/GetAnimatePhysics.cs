using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimation
{
	[TaskCategory("Basic/Animation")]
	[TaskDescription("Stores the animate physics value. Returns Success.")]
	public class GetAnimatePhysics : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("Are the if animations are executed in the physics loop?")]
		[RequiredField]
		public SharedBool storeValue;

		private Animation animation;

		private GameObject prevGameObject;

		public GetAnimatePhysics()
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
			storeValue.set_Value(animation.get_animatePhysics());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			storeValue.set_Value(false);
		}
	}
}
