using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityTransform
{
	[TaskCategory("Basic/Transform")]
	[TaskDescription("Stores the parent of the Transform. Returns Success.")]
	public class GetParent : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The parent of the Transform")]
		[RequiredField]
		public SharedTransform storeValue;

		private Transform targetTransform;

		private GameObject prevGameObject;

		public GetParent()
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
			if (targetTransform == null)
			{
				Debug.LogWarning((object)"Transform is null");
				return 1;
			}
			storeValue.set_Value(targetTransform.get_parent());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			storeValue = null;
		}
	}
}
