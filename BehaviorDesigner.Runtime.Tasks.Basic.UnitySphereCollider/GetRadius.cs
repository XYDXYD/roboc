using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnitySphereCollider
{
	[TaskCategory("Basic/SphereCollider")]
	[TaskDescription("Stores the radius of the SphereCollider. Returns Success.")]
	public class GetRadius : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The radius of the SphereCollider")]
		[RequiredField]
		public SharedFloat storeValue;

		private SphereCollider sphereCollider;

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
			storeValue.set_Value(sphereCollider.get_radius());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			storeValue = 0f;
		}
	}
}
