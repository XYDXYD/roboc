using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityTransform
{
	[TaskCategory("Basic/Transform")]
	[TaskDescription("Stores the forward vector of the Transform. Returns Success.")]
	public class GetForwardVector : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The position of the Transform")]
		[RequiredField]
		public SharedVector3 storeValue;

		private Transform targetTransform;

		private GameObject prevGameObject;

		public GetForwardVector()
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
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			if (targetTransform == null)
			{
				Debug.LogWarning((object)"Transform is null");
				return 1;
			}
			storeValue.set_Value(targetTransform.get_forward());
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
