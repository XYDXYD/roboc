using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimator
{
	[TaskCategory("Basic/Animator")]
	[TaskDescription("Gets the avatar delta rotation for the last evaluated frame. Returns Success.")]
	public class GetDeltaRotation : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The avatar delta rotation")]
		[RequiredField]
		public SharedQuaternion storeValue;

		private Animator animator;

		private GameObject prevGameObject;

		public GetDeltaRotation()
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
			storeValue.set_Value(animator.get_deltaRotation());
			return 2;
		}

		public override void OnReset()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			if (storeValue != null)
			{
				storeValue.set_Value(Quaternion.get_identity());
			}
		}
	}
}
