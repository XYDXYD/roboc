using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityParticleSystem
{
	[TaskCategory("Basic/ParticleSystem")]
	[TaskDescription("Is the Particle System playing?")]
	public class IsPlaying : Conditional
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		private ParticleSystem particleSystem;

		private GameObject prevGameObject;

		public IsPlaying()
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
			if (particleSystem == null)
			{
				Debug.LogWarning((object)"ParticleSystem is null");
				return 1;
			}
			return (!particleSystem.get_isPlaying()) ? 1 : 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
		}
	}
}
