using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnitySphereCollider
{
	[TaskCategory("Basic/SphereCollider")]
	[TaskDescription("Sets the radius of the SphereCollider. Returns Success.")]
	public class SetRadius : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The radius of the SphereCollider")]
		public SharedFloat radius;

		private SphereCollider sphereCollider;

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
				sphereCollider = defaultGameObject.GetComponent<SphereCollider>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (sphereCollider == null)
			{
				Debug.LogWarning((object)"SphereCollider is null");
				return 1;
			}
			sphereCollider.set_radius(radius.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			radius = 0f;
		}
	}
}
