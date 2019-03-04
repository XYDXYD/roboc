using Svelto.IoC;
using UnityEngine;

namespace Simulation
{
	internal class NanoBeamData : MonoBehaviour
	{
		public string AudioCubeDestroyed = "NanoDisruptor_DisruptorHit";

		public string AudioCubeHealed = "NanoDisruptor_DisruptorHealCube";

		public GameObject RespawnEffectPrefab;

		[Inject]
		internal NanoBeamAudioManager manager
		{
			private get;
			set;
		}

		public NanoBeamData()
			: this()
		{
		}

		private void Start()
		{
			manager.Init(AudioCubeDestroyed, AudioCubeHealed, RespawnEffectPrefab);
		}
	}
}
