using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityParticleSystem
{
	[TaskCategory("Basic/ParticleSystem")]
	[TaskDescription("Enables or disables the Particle System emission.")]
	public class SetEnableEmission : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("Enable the ParticleSystem emissions?")]
		public SharedBool enable;

		private ParticleSystem particleSystem;

		private GameObject prevGameObject;

		public SetEnableEmission()
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
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			if (particleSystem == null)
			{
				Debug.LogWarning((object)"ParticleSystem is null");
				return 1;
			}
			EmissionModule emission = particleSystem.get_emission();
			emission.set_enabled(enable.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			enable = false;
		}
	}
}
