using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimator
{
	[TaskCategory("Basic/Animator")]
	[TaskDescription("Gets the avatar delta position for the last evaluated frame. Returns Success.")]
	public class GetDeltaPosition : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The avatar delta position")]
		[RequiredField]
		public SharedVector3 storeValue;

		private Animator animator;

		private GameObject prevGameObject;

		public GetDeltaPosition()
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
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			if (animator == null)
			{
				Debug.LogWarning((object)"Animator is null");
				return 1;
			}
			storeValue.set_Value(animator.get_deltaPosition());
			return 2;
		}

		public override void OnReset()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			targetGameObject = null;
			storeValue = Vector3.get_zero();
		}
	}
}
