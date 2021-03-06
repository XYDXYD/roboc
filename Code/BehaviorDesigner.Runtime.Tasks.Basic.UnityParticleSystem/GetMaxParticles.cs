using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityParticleSystem
{
	[TaskCategory("Basic/ParticleSystem")]
	[TaskDescription("Stores the max particles of the Particle System.")]
	public class GetMaxParticles : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The max particles of the ParticleSystem")]
		[RequiredField]
		public SharedFloat storeResult;

		private ParticleSystem particleSystem;

		private GameObject prevGameObject;

		public GetMaxParticles()
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
			SharedFloat sharedFloat = storeResult;
			MainModule main = particleSystem.get_main();
			sharedFloat.set_Value((float)main.get_maxParticles());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			storeResult = 0f;
		}
	}
}
