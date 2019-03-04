using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimator
{
	[TaskCategory("Basic/Animator")]
	[TaskDescription("Plays an animator state. Returns Success.")]
	public class Play : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The name of the state")]
		public SharedString stateName;

		[Tooltip("The layer where the state is")]
		public int layer = -1;

		[Tooltip("The normalized time at which the state will play")]
		public float normalizedTime = float.NegativeInfinity;

		private Animator animator;

		private GameObject prevGameObject;

		public Play()
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
			animator.Play(stateName.get_Value(), layer, normalizedTime);
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			stateName = string.Empty;
			layer = -1;
			normalizedTime = float.NegativeInfinity;
		}
	}
}
