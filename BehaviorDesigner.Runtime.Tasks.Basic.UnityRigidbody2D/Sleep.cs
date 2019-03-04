using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityRigidbody2D
{
	[TaskCategory("Basic/Rigidbody2D")]
	[TaskDescription("Forces the Rigidbody2D to sleep at least one frame. Returns Success.")]
	public class Sleep : Conditional
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		private Rigidbody2D rigidbody2D;

		private GameObject prevGameObject;

		public Sleep()
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
			rigidbody2D.Sleep();
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
		}
	}
}
