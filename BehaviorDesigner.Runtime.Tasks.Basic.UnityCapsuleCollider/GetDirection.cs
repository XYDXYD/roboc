using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityCapsuleCollider
{
	[TaskCategory("Basic/CapsuleCollider")]
	[TaskDescription("Stores the direction of the CapsuleCollider. Returns Success.")]
	public class GetDirection : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The direction of the CapsuleCollider")]
		[RequiredField]
		public SharedInt storeValue;

		private CapsuleCollider capsuleCollider;

		private GameObject prevGameObject;

		public GetDirection()
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
			storeValue.set_Value(capsuleCollider.get_direction());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			storeValue = 0;
		}
	}
}
