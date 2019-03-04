using UnityEngine;

namespace Simulation
{
	internal class PlaySoundWhenParticleDestroyedImplementor : MonoBehaviour, IPlaySoundWhenParticleDestroyedComponent, IImplementor
	{
		[SerializeField]
		private ParticleSystem _particleSys;

		[SerializeField]
		private string _soundToPlay;

		public ParticleSystem particleSys => _particleSys;

		public string soundToPlay => _soundToPlay;

		public int previousParticleNumber
		{
			get;
			set;
		}

		public PlaySoundWhenParticleDestroyedImplementor()
			: this()
		{
		}
	}
}
