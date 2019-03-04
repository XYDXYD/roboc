using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityTransform
{
	[TaskCategory("Basic/Transform")]
	[TaskDescription("Rotates the transform so the forward vector points at worldPosition. Returns Success.")]
	public class LookAt : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The GameObject to look at. If null the world position will be used.")]
		public SharedGameObject targetLookAt;

		[Tooltip("Point to look at")]
		public SharedVector3 worldPosition;

		[Tooltip("Vector specifying the upward direction")]
		public Vector3 worldUp;

		private Transform targetTransform;

		private GameObject prevGameObject;

		public LookAt()
			: this()
		{
		}

		public override void OnStart()
		{
			GameObject defaultGameObject = this.GetDefaultGameObject(targetGameObject.get_Value());
			if (defaultGameObject != prevGameObject)
			{
				targetTransform = defaultGameObject.GetComponent<Transform>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			if (targetTransform == null)
			{
				Debug.LogWarning((object)"Transform is null");
				return 1;
			}
			if (targetLookAt.get_Value() != null)
			{
				targetTransform.LookAt(targetLookAt.get_Value().get_transform());
			}
			else
			{
				targetTransform.LookAt(worldPosition.get_Value(), worldUp);
			}
			return 2;
		}

		public override void OnReset()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			targetGameObject = null;
			targetLookAt = null;
			worldPosition = Vector3.get_up();
			worldUp = Vector3.get_up();
		}
	}
}
