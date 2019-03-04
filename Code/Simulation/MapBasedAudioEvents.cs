using UnityEngine;

namespace Simulation
{
	internal class MapBasedAudioEvents : MonoBehaviour
	{
		[SerializeField]
		private string gameStart;

		public string GameStart => gameStart;

		public MapBasedAudioEvents()
			: this()
		{
		}

		private void Start()
		{
		}
	}
}
