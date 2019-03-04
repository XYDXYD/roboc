using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityParticleSystem
{
	[TaskCategory("Basic/ParticleSystem")]
	[TaskDescription("Stores if the Particle System is emitting particles.")]
	public class GetEnableEmission : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("Is the Particle System emitting particles?")]
		[RequiredField]
		public SharedBool storeResult;

		private ParticleSystem particleSystem;

		private GameObject prevGameObject;

		public GetEnableEmission()
			: this()
		{
		}

		public override void OnStart()
		{
			GameObject defaultGameObject = this.GetDefaultGameObject(targetGameObject.get_Value());
			if (defaultGameObject != prevGameObject)
			{
				particleSystem = defaultGameObject.GetComponent<ParticleSystem>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			if (particleSystem == null)
			{
				Debug.LogWarning((object)"ParticleSystem is null");
				return 1;
			}
			SharedBool sharedBool = storeResult;
			EmissionModule emission = particleSystem.get_emission();
			sharedBool.set_Value(emission.get_enabled());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			storeResult = false;
		}
	}
}
