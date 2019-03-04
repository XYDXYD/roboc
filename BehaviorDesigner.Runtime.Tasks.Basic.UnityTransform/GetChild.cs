using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityTransform
{
	[TaskCategory("Basic/Transform")]
	[TaskDescription("Stores the transform child at the specified index. Returns Success.")]
	public class GetChild : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The index of the child")]
		public SharedInt index;

		[Tooltip("The child of the Transform")]
		[RequiredField]
		public SharedTransform storeValue;

		private Transform targetTransform;

		private GameObject prevGameObject;

		public GetChild()
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
			storeValue.set_Value(targetTransform.GetChild(index.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			index = 0;
			storeValue = null;
		}
	}
}
