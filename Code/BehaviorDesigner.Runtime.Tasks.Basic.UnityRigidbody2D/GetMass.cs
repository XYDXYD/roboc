using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityRigidbody2D
{
	[TaskCategory("Basic/Rigidbody2D")]
	[TaskDescription("Stores the mass of the Rigidbody2D. Returns Success.")]
	public class GetMass : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The mass of the Rigidbody2D")]
		[RequiredField]
		public SharedFloat storeValue;

		private Rigidbody2D rigidbody2D;

		private GameObject prevGameObject;

		public GetMass()
			: this()
		{
		}

		public override void OnStart()
		{
			GameObject defaultGameObject = this.GetDefaultGameObject(targetGameObject.get_Value());
			if (defaultGameObject != prevGameObject)
			{
				rigidbody2D = defaultGameObject.GetComponent<Rigidbody2D>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (rigidbody2D == null)
			{
				Debug.LogWarning((object)"Rigidbody2D is null");
				return 1;
			}
			storeValue.set_Value(rigidbody2D.get_mass());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			storeValue = 0f;
		}
	}
}
