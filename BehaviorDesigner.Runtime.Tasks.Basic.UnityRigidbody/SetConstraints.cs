using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityRigidbody
{
	[TaskCategory("Basic/Rigidbody")]
	[TaskDescription("Sets the constraints of the Rigidbody. Returns Success.")]
	public class SetConstraints : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The constraints of the Rigidbody")]
		public RigidbodyConstraints constraints;

		private Rigidbody rigidbody;

		private GameObject prevGameObject;

		public SetConstraints()
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
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			if (rigidbody == null)
			{
				Debug.LogWarning((object)"Rigidbody is null");
				return 1;
			}
			rigidbody.set_constraints(constraints);
			return 2;
		}

		public override void OnReset()
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			targetGameObject = null;
			constraints = 0;
		}
	}
}
