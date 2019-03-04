using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityRigidbody
{
	[TaskCategory("Basic/Rigidbody")]
	[TaskDescription("Applies a force to the rigidbody that simulates explosion effects. Returns Success.")]
	public class AddExplosionForce : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The force of the explosion")]
		public SharedFloat explosionForce;

		[Tooltip("The position of the explosion")]
		public SharedVector3 explosionPosition;

		[Tooltip("The radius of the explosion")]
		public SharedFloat explosionRadius;

		[Tooltip("Applies the force as if it was applied from beneath the object")]
		public float upwardsModifier;

		[Tooltip("The type of force")]
		public ForceMode forceMode;

		private Rigidbody rigidbody;

		private GameObject prevGameObject;

		public AddExplosionForce()
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
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			if (rigidbody == null)
			{
				Debug.LogWarning((object)"Rigidbody is null");
				return 1;
			}
			rigidbody.AddExplosionForce(explosionForce.get_Value(), explosionPosition.get_Value(), explosionRadius.get_Value(), upwardsModifier, forceMode);
			return 2;
		}

		public override void OnReset()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			targetGameObject = null;
			explosionForce = 0f;
			explosionPosition = Vector3.get_zero();
			explosionRadius = 0f;
			upwardsModifier = 0f;
			forceMode = 0;
		}
	}
}
