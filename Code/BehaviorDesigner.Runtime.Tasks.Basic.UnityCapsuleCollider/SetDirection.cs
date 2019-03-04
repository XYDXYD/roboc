using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityCapsuleCollider
{
	[TaskCategory("Basic/CapsuleCollider")]
	[TaskDescription("Sets the direction of the CapsuleCollider. Returns Success.")]
	public class SetDirection : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The direction of the CapsuleCollider")]
		public SharedInt direction;

		private CapsuleCollider capsuleCollider;

		private GameObject prevGameObject;

		public SetDirection()
			: this()
		{
		}

		public override void OnStart()
		{
			GameObject defaultGameObject = this.GetDefaultGameObject(targetGameObject.get_Value());
			if (defaultGameObject != prevGameObject)
			{
				capsuleCollider = defaultGameObject.GetComponent<CapsuleCollider>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (capsuleCollider == null)
			{
				Debug.LogWarning((object)"CapsuleCollider is null");
				return 1;
			}
			capsuleCollider.set_direction(direction.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			direction = 0;
		}
	}
}
