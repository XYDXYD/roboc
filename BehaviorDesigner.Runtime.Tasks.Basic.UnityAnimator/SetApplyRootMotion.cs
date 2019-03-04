using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimator
{
	[TaskCategory("Basic/Animator")]
	[TaskDescription("Sets if root motion is applied. Returns Success.")]
	public class SetApplyRootMotion : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("Is root motion applied?")]
		public SharedBool rootMotion;

		private Animator animator;

		private GameObject prevGameObject;

		public SetApplyRootMotion()
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
			animator.set_applyRootMotion(rootMotion.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			rootMotion = false;
		}
	}
}
