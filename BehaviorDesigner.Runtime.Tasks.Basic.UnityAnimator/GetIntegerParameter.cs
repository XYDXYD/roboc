using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimator
{
	[TaskCategory("Basic/Animator")]
	[TaskDescription("Stores the integer parameter on an animator. Returns Success.")]
	public class GetIntegerParameter : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The name of the parameter")]
		public SharedString paramaterName;

		[Tooltip("The value of the integer parameter")]
		[RequiredField]
		public SharedInt storeValue;

		private Animator animator;

		private GameObject prevGameObject;

		public GetIntegerParameter()
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
			storeValue.set_Value(animator.GetInteger(paramaterName.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			paramaterName = string.Empty;
			storeValue = 0;
		}
	}
}
