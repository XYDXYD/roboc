using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimator
{
	[TaskCategory("Basic/Animator")]
	[TaskDescription("Sets the layer's current weight. Returns Success.")]
	public class SetLayerWeight : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The layer's index")]
		public SharedInt index;

		[Tooltip("The weight of the layer")]
		public SharedFloat weight;

		private Animator animator;

		private GameObject prevGameObject;

		public SetLayerWeight()
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
			animator.SetLayerWeight(index.get_Value(), weight.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			index = 0;
			weight = 0f;
		}
	}
}
