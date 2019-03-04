using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityCircleCollider2D
{
	[TaskCategory("Basic/CircleCollider2D")]
	[TaskDescription("Stores the radius of the CircleCollider2D. Returns Success.")]
	public class GetRadius : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The radius of the CircleCollider2D")]
		[RequiredField]
		public SharedFloat storeValue;

		private CircleCollider2D circleCollider2D;

		private GameObject prevGameObject;

		public GetRadius()
			: this()
		{
		}

		public override void OnStart()
		{
			GameObject defaultGameObject = this.GetDefaultGameObject(targetGameObject.get_Value());
			if (defaultGameObject != prevGameObject)
			{
				circleCollider2D = defaultGameObject.GetComponent<CircleCollider2D>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (circleCollider2D == null)
			{
				Debug.LogWarning((object)"CircleCollider2D is null");
				return 1;
			}
			storeValue.set_Value(circleCollider2D.get_radius());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			storeValue = 0f;
		}
	}
}
