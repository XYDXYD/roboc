using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityParticleSystem
{
	[TaskCategory("Basic/ParticleSystem")]
	[TaskDescription("Sets if the Particle System should loop.")]
	public class SetLoop : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("Should the ParticleSystem loop?")]
		public SharedBool loop;

		private ParticleSystem particleSystem;

		private GameObject prevGameObject;

		public SetLoop()
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
			MainModule main = particleSystem.get_main();
			main.set_loop(loop.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			loop = false;
		}
	}
}
