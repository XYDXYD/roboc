using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimator
{
	[TaskCategory("Basic/Animator")]
	[TaskDescription("Interrupts the automatic target matching. Returns Success.")]
	public class InterruptMatchTarget : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("CompleteMatch will make the gameobject match the target completely at the next frame")]
		public bool completeMatch = true;

		private Animator animator;

		private GameObject prevGameObject;

		public InterruptMatchTarget()
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
			animator.InterruptMatchTarget(completeMatch);
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			completeMatch = true;
		}
	}
}
