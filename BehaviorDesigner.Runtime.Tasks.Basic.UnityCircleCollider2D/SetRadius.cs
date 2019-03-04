using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityCircleCollider2D
{
	[TaskCategory("Basic/CircleCollider2D")]
	[TaskDescription("Sets the radius of the CircleCollider2D. Returns Success.")]
	public class SetRadius : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The radius of the CircleCollider2D")]
		public SharedFloat radius;

		private CircleCollider2D circleCollider2D;

		private GameObject prevGameObject;

		public SetRadius()
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
			circleCollider2D.set_radius(radius.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			radius = 0f;
		}
	}
}
