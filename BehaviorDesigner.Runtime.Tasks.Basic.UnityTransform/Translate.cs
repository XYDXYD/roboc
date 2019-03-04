using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityTransform
{
	[TaskCategory("Basic/Transform")]
	[TaskDescription("Moves the transform in the direction and distance of translation. Returns Success.")]
	public class Translate : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("Move direction and distance")]
		public SharedVector3 translation;

		[Tooltip("Specifies which axis the rotation is relative to")]
		public Space relativeTo = 1;

		private Transform targetTransform;

		private GameObject prevGameObject;

		public Translate()
			: this()
		{
		}//IL_0002: Unknown result type (might be due to invalid IL or missing references)


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
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			if (targetTransform == null)
			{
				Debug.LogWarning((object)"Transform is null");
				return 1;
			}
			targetTransform.Translate(translation.get_Value(), relativeTo);
			return 2;
		}

		public override void OnReset()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			targetGameObject = null;
			translation = Vector3.get_zero();
			relativeTo = 1;
		}
	}
}
