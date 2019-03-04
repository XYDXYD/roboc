using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimator
{
	[TaskCategory("Basic/Animator")]
	[TaskDescription("Returns success if the specified name matches the name of the active state.")]
	public class IsName : Conditional
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The layer's index")]
		public SharedInt index;

		[Tooltip("The state name to compare")]
		public SharedString name;

		private Animator animator;

		private GameObject prevGameObject;

		public IsName()
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
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			if (animator == null)
			{
				Debug.LogWarning((object)"Animator is null");
				return 1;
			}
			AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(index.get_Value());
			return (!currentAnimatorStateInfo.IsName(name.get_Value())) ? 1 : 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			index = 0;
			name = string.Empty;
		}
	}
}
