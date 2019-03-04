using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityCapsuleCollider
{
	[TaskCategory("Basic/CapsuleCollider")]
	[TaskDescription("Sets the height of the CapsuleCollider. Returns Success.")]
	public class SetHeight : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The height of the CapsuleCollider")]
		public SharedFloat direction;

		private CapsuleCollider capsuleCollider;

		private GameObject prevGameObject;

		public SetHeight()
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
			capsuleCollider.set_height(direction.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			direction = 0f;
		}
	}
}
