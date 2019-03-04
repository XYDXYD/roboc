using UnityEngine;

namespace Simulation.Hardware.Cosmetic
{
	public class TogglableCosmeticParticlesImplementor : MonoBehaviour, ITogglableCosmeticParticlesComponent, ICosmeticRenderLimitCubeComponent
	{
		[SerializeField]
		private ParticleSystem _idleLoopParticles;

		[SerializeField]
		private ParticleSystem _stopParticles;

		public ParticleSystem idleLoopParticles => _idleLoopParticles;

		public ParticleSystem stopParticles => _stopParticles;

		public bool isAboveCosmeticRenderLimit
		{
			get;
			set;
		}

		public TogglableCosmeticParticlesImplementor()
			: this()
		{
		}
	}
}
