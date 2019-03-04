using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimator
{
	[TaskCategory("Basic/Animator")]
	[TaskDescription("Sets the look at position. Returns Success.")]
	public class SetLookAtPosition : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The position to lookAt")]
		public SharedVector3 position;

		private Animator animator;

		private GameObject prevGameObject;

		private bool positionSet;

		public SetLookAtPosition()
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
			positionSet = false;
		}

		public override TaskStatus OnUpdate()
		{
			if (animator == null)
			{
				Debug.LogWarning((object)"Animator is null");
				return 1;
			}
			return (!positionSet) ? 3 : 2;
		}

		public override void OnAnimatorIK()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			if (!(animator == null))
			{
				animator.SetLookAtPosition(position.get_Value());
				positionSet = true;
			}
		}

		public override void OnReset()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			targetGameObject = null;
			position = Vector3.get_zero();
		}
	}
}
