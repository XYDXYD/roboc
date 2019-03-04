using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityCapsuleCollider
{
	[TaskCategory("Basic/CapsuleCollider")]
	[TaskDescription("Stores the center of the CapsuleCollider. Returns Success.")]
	public class GetCenter : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The center of the CapsuleCollider")]
		[RequiredField]
		public SharedVector3 storeValue;

		private CapsuleCollider capsuleCollider;

		private GameObject prevGameObject;

		public GetCenter()
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
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			if (capsuleCollider == null)
			{
				Debug.LogWarning((object)"CapsuleCollider is null");
				return 1;
			}
			storeValue.set_Value(capsuleCollider.get_center());
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
