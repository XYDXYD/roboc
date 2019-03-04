using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityBoxCollider
{
	[TaskCategory("Basic/BoxCollider")]
	[TaskDescription("Sets the center of the BoxCollider. Returns Success.")]
	public class SetCenter : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The center of the BoxCollider")]
		public SharedVector3 center;

		private BoxCollider boxCollider;

		private GameObject prevGameObject;

		public SetCenter()
			: this()
		{
		}

		public override void OnStart()
		{
			GameObject defaultGameObject = this.GetDefaultGameObject(targetGameObject.get_Value());
			if (defaultGameObject != prevGameObject)
			{
				boxCollider = defaultGameObject.GetComponent<BoxCollider>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			if (boxCollider == null)
			{
				Debug.LogWarning((object)"BoxCollider is null");
				return 1;
			}
			boxCollider.set_center(center.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			targetGameObject = null;
			center = Vector3.get_zero();
		}
	}
}
