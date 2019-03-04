using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimator
{
	[TaskCategory("Basic/Animator")]
	[TaskDescription("Waits for the Animator to reach the specified state.")]
	public class WaitForState : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The name of the state")]
		public SharedString stateName;

		[Tooltip("The layer where the state is")]
		public SharedInt layer = -1;

		private Animator animator;

		private GameObject prevGameObject;

		private int stateHash;

		public WaitForState()
			: this()
		{
		}

		public override void OnAwake()
		{
			stateHash = Animator.StringToHash(stateName.get_Value());
		}

		public override void OnStart()
		{
			GameObject defaultGameObject = this.GetDefaultGameObject(targetGameObject.get_Value());
			if (defaultGameObject != prevGameObject)
			{
				animator = defaultGameObject.GetComponent<Animator>();
				prevGameObject = defaultGameObject;
				if (!animator.HasState(layer.get_Value(), stateHash))
				{
					Debug.LogError((object)("Error: The Animator does not have the state " + stateName.get_Value() + " on layer " + layer.get_Value()));
				}
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
			AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(layer.get_Value());
			if (currentAnimatorStateInfo.get_fullPathHash() != stateHash)
			{
				return 3;
			}
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			stateName = string.Empty;
			layer = -1;
		}
	}
}
