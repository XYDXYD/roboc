using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityRigidbody
{
	[TaskCategory("Basic/Rigidbody")]
	[TaskDescription("Returns Success if the Rigidbody is using gravity, otherwise Failure.")]
	public class UseGravity : Conditional
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		private Rigidbody rigidbody;

		private GameObject prevGameObject;

		public UseGravity()
			: this()
		{
		}

		public override void OnStart()
		{
			GameObject defaultGameObject = this.GetDefaultGameObject(targetGameObject.get_Value());
			if (defaultGameObject != prevGameObject)
			{
				rigidbody = defaultGameObject.GetComponent<Rigidbody>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (rigidbody == null)
			{
				Debug.LogWarning((object)"Rigidbody is null");
				return 1;
			}
			return (!rigidbody.get_useGravity()) ? 1 : 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
		}
	}
}
