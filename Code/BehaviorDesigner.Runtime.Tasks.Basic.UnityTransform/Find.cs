using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityTransform
{
	[TaskCategory("Basic/Transform")]
	[TaskDescription("Finds a transform by name. Returns Success.")]
	public class Find : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The transform name to find")]
		public SharedString transformName;

		[Tooltip("The object found by name")]
		[RequiredField]
		public SharedTransform storeValue;

		private Transform targetTransform;

		private GameObject prevGameObject;

		public Find()
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
			storeValue.set_Value(targetTransform.Find(transformName.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			transformName = null;
			storeValue = null;
		}
	}
}
