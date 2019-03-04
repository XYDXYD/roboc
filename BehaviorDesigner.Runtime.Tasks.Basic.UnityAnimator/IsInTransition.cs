using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimator
{
	[TaskCategory("Basic/Animator")]
	[TaskDescription("Returns success if the specified AnimatorController layer in a transition.")]
	public class IsInTransition : Conditional
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The layer's index")]
		public SharedInt index;

		private Animator animator;

		private GameObject prevGameObject;

		public IsInTransition()
			: this()
		{
		}

		public override void OnStart()
		{
			GameObject defaultGameObject = this.GetDefaultGameObject(targetGameObject.get_Value());
			if (defaultGameObject != prevGameObject)
			{
				animator = defaultGameObject.GetComponent<Animator>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (animator == null)
			{
				Debug.LogWarning((object)"Animator is null");
				return 1;
			}
			return (!animator.IsInTransition(index.get_Value())) ? 1 : 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			index = 0;
		}
	}
}
